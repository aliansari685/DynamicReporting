import { Metadata, urls } from "./state.js";
import { escapeHtml, showNotification } from "./ui.js";

export function openCreateModal() {

    document.getElementById("editorTitle").textContent =
        "ایجاد گزارش";

    document.getElementById("editingReportId").value =
        "";

    document.getElementById("reportName").value =
        "";

    document.getElementById("baseTable").value =
        "";

    document.getElementById("isDefault").checked =
        false;

    renderColumns();

    document
        .getElementById("reportEditorModal")
        .showModal();

}


/*
 * =====================================================
 * Edit
 * =====================================================
 */

export async function openEditModal(id) {

    const response =
        await fetch(
            `${urls.get}?id=${id}`);

    if (!response.ok) {

        showNotification(
            "دریافت اطلاعات گزارش انجام نشد.",
            "error");

        return;
    }

    const report =
        await response.json();

    document.getElementById("editorTitle").textContent =
        "ویرایش گزارش";

    document.getElementById("editingReportId").value =
        report.id;

    document.getElementById("reportName").value =
        report.name;

    document.getElementById("baseTable").value =
        report.baseTable;

    document.getElementById("isDefault").checked =
        report.isDefault;

    renderColumns(
        report.selectedColumns || []);

    document
        .getElementById("reportEditorModal")
        .showModal();

}


/*
 * =====================================================
 * Columns
 * =====================================================
 */

document
    .getElementById("baseTable")
    .addEventListener(
        "change",
        () => renderColumns()
    );


export function renderColumns(selected = []) {

    const table =
        document.getElementById("baseTable").value;

    const container =
        document.getElementById("columnsContainer");

    container.innerHTML = "";

    if (!table) {

        container.innerHTML = `
                            <div class="py-6 text-center text-sm text-base-content/50 md:col-span-2">
                                ابتدا جدول پایه را انتخاب کنید.
                            </div>`;

        updateSelectedCount();

        return;
    }


    const metadataTable =
        Metadata.find(x =>
            x.tableName === table);


    if (!metadataTable) {

        container.innerHTML = `
                            <div class="py-6 text-center text-sm text-error md:col-span-2">
                                متادیتای این جدول پیدا نشد.
                            </div>`;

        return;
    }


    metadataTable.columns.forEach(column => {

        const checked =
            selected.some(x =>
                x.table === table &&
                x.column === column.physicalName);


        const wrapper =
            document.createElement("label");


        wrapper.className =
            "flex cursor-pointer items-center gap-3 rounded-lg border border-base-300 p-3 hover:bg-base-200";


        wrapper.innerHTML = `
                            <input
                                type="checkbox"
                                class="checkbox checkbox-primary report-column"
                                data-table="${escapeHtml(table)}"
                                data-column="${escapeHtml(column.physicalName)}"
                                ${checked ? "checked" : ""} />

                            <span class="flex flex-col">

                                <span class="font-medium">
                                    ${escapeHtml(column.displayName || column.physicalName)}
                                </span>

                                <span class="text-xs text-base-content/50">
                                    ${escapeHtml(column.physicalName)}
                                </span>

                            </span>
                        `;


        container.appendChild(wrapper);

    });


    document
        .querySelectorAll(".report-column")
        .forEach(x =>
            x.addEventListener(
                "change",
                updateSelectedCount));


    updateSelectedCount();

}


export function updateSelectedCount() {

    const count =
        document.querySelectorAll(
            ".report-column:checked").length;

    const element =
        document.getElementById(
            "selectedColumnCount");

    if (element)
        element.textContent = count;

}


/*
 * =====================================================
 * Save
 * =====================================================
 */

document
    .getElementById("reportEditorForm")
    .addEventListener(
        "submit",
        async function (event) {

            event.preventDefault();

            const id =
                document.getElementById(
                    "editingReportId").value;


            const selectedColumns =
                Array.from(
                    document.querySelectorAll(
                        ".report-column:checked"))
                    .map(x => ({
                        table: x.dataset.table,
                        column: x.dataset.column
                    }));


            const model = {
                name:
                    document.getElementById(
                        "reportName").value,

                baseTable:
                    document.getElementById(
                        "baseTable").value,

                isDefault:
                    document.getElementById(
                        "isDefault").checked,

                createdBy:
                    "MVC",

                selectedColumns:
                    selectedColumns

            };


            const token =
                document.querySelector(
                    'input[name="__RequestVerificationToken"]')
                    ?.value;


            const isEdit =
                id !== "";


            const url =
                isEdit
                    ? `${urls.update}?id=${id}`
                    : urls.create;


            const response =
                await fetch(
                    url,
                    {
                        method:
                            isEdit
                                ? "PUT"
                                : "POST",

                        headers: {
                            'Content-Type':
                                "application/json",

                            ...(token
                                ? {
                                    'RequestVerificationToken':
                                        token
                                }
                                : {})

                        },

                        body:
                            JSON.stringify(model)

                    });


            if (!response.ok) {

                showNotification(
                    "ذخیره گزارش انجام نشد.",
                    "error");

                return;

            }


            showNotification(
                isEdit
                    ? "گزارش با موفقیت ویرایش شد."
                    : "گزارش با موفقیت ایجاد شد.",
                "success");


            document
                .getElementById("reportEditorModal")
                .close();


            setTimeout(
                () => location.reload(),
                600);

        });



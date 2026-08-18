import { Metadata, urls } from "./state.js";
import { escapeHtml, showNotification } from "./ui.js";


/*
 * =====================================================
 * Create
 * =====================================================
 */

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

    const search =
        document.getElementById("columnSearch");

    if (search)
        search.value = "";

    renderColumns([]);

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

    const search =
        document.getElementById("columnSearch");

    if (search)
        search.value = "";

    renderColumns(
        report.selectedColumns || []);


    document
        .getElementById("reportEditorModal")
        .showModal();

}


/*
 * =====================================================
 * Base Table
 * =====================================================
 */

document
    .getElementById("baseTable")
    .addEventListener(
        "change",
        () => {

            renderColumns(
                getSelectedColumns()
            );

        }
    );


/*
 * =====================================================
 * Render Columns
 * =====================================================
 */

export function renderColumns(selected = []) {

    const container =
        document.getElementById(
            "columnsContainer");

    container.innerHTML = "";


    if (!Metadata || !Metadata.length) {

        container.innerHTML = `
            <div class="py-8 text-center text-sm text-error">
                متادیتای جداول دریافت نشده است.
            </div>`;

        updateSelectedCount();

        return;
    }

    Metadata.forEach(table => {

        const selectedForTable =
            selected.filter(
                x =>
                    (x.table ?? x.Table) === table.TableName);

        const group =
            createTableGroup(
                table,
                selectedForTable);

        container.appendChild(group);

    });


    bindColumnEvents();

    updateSelectedCount();

}


/*
 * =====================================================
 * Table Group
 * =====================================================
 */

function createTableGroup(
    table,
    selectedColumns) {

    const wrapper =
        document.createElement("div");

    wrapper.className =
        "overflow-hidden rounded-xl border border-base-300 bg-base-100";


    const columns =
        table.Columns ?? [];


    const selectedCount =
        selectedColumns.length;


    const allSelected =
        columns.length > 0 &&
        selectedCount === columns.length;


    wrapper.innerHTML = `

        <div class="flex items-center gap-2 border-b border-base-300 px-3 py-2">

            <button
                type="button"
                class="table-toggle btn btn-sm btn-ghost"
                aria-expanded="true">

                <svg
                    class="table-chevron h-4 w-4 transition-transform"
                    xmlns="http://www.w3.org/2000/svg"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor">

                    <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="m6 9 6 6 6-6" />

                </svg>

            </button>


            <div class="min-w-0 flex-1">

                <div class="flex items-center gap-2">

                    <span class="font-bold">

                        ${escapeHtml(
        table.DisplayName ||
        table.TableName)}

                    </span>

                    <span class="badge badge-ghost badge-sm">

                        ${columns.length} ستون

                    </span>

                </div>


                <div class="text-xs text-base-content/40">

                    ${escapeHtml(table.TableName)}

                </div>

            </div>


            <label class="flex cursor-pointer items-center gap-2 text-xs">

                <span class="hidden sm:inline">
                    انتخاب همه
                </span>

                <input
                    type="checkbox"
                    class="checkbox checkbox-sm checkbox-primary table-select-all"
                    data-table="${escapeHtml(table.TableName)}"
                    ${allSelected ? "checked" : ""} />

            </label>

        </div>


        <div class="table-columns grid gap-2 p-3 sm:grid-cols-2">

            ${columns.map(column => {

            const checked =
                selectedColumns.some(x =>
                    (x.column ?? x.Column) === column.PhysicalName);


            return `

                    <label
                        class="column-item flex cursor-pointer items-center gap-3 rounded-lg border border-base-300 p-3 transition hover:bg-base-200">

                        <input
                            type="checkbox"
                            class="checkbox checkbox-primary report-column"
                            data-table="${escapeHtml(table.TableName)}"
                            data-column="${escapeHtml(column.PhysicalName)}"
                            ${checked ? "checked" : ""} />

                        <span class="min-w-0 flex flex-col">

                            <span class="truncate font-medium">

                                ${escapeHtml(
                column.DisplayName ||
                column.PhysicalName)}

                            </span>

                            <span class="truncate text-xs text-base-content/40">

                                ${escapeHtml(
                    column.PhysicalName)}

                            </span>

                        </span>

                    </label>

                `;

        }).join("")}

        </div>

    `;


    return wrapper;
}


/*
 * =====================================================
 * Column Events
 * =====================================================
 */

function bindColumnEvents() {

    /*
     * Individual columns
     */

    document
        .querySelectorAll(".report-column")
        .forEach(checkbox => {

            checkbox.addEventListener(
                "change",
                updateSelectedCount);

        });


    /*
     * Select all per table
     */

    document
        .querySelectorAll(".table-select-all")
        .forEach(selectAll => {

            selectAll.addEventListener(
                "change",
                function () {

                    const table =
                        this.dataset.table;

                    document
                        .querySelectorAll(
                            `.report-column[data-table="${CSS.escape(table)}"]`)
                        .forEach(column => {

                            column.checked =
                                this.checked;

                        });

                    updateSelectedCount();

                });

        });


    /*
     * Expand / Collapse
     */

    document
        .querySelectorAll(".table-toggle")
        .forEach(toggle => {

            toggle.addEventListener(
                "click",
                function () {

                    const group =
                        this.closest(
                            ".overflow-hidden");

                    const columns =
                        group.querySelector(
                            ".table-columns");

                    const chevron =
                        group.querySelector(
                            ".table-chevron");

                    const expanded =
                        this.getAttribute(
                            "aria-expanded") ===
                        "true";


                    this.setAttribute(
                        "aria-expanded",
                        String(!expanded));


                    columns.classList.toggle(
                        "hidden",
                        expanded);


                    chevron.classList.toggle(
                        "rotate-180",
                        expanded);

                });

        });


    /*
     * Search
     */

    const search =
        document.getElementById(
            "columnSearch");


    if (search) {

        search.oninput =
            filterColumns;

    }


    /*
     * Global select
     */

    const selectAll =
        document.getElementById(
            "selectAllColumns");


    if (selectAll) {

        selectAll.onclick =
            () => {

                document
                    .querySelectorAll(
                        ".report-column")
                    .forEach(x =>
                        x.checked = true);

                document
                    .querySelectorAll(
                        ".table-select-all")
                    .forEach(x =>
                        x.checked = true);

                updateSelectedCount();

            };

    }


    /*
     * Global clear
     */

    const clearAll =
        document.getElementById(
            "clearAllColumns");


    if (clearAll) {

        clearAll.onclick =
            () => {

                document
                    .querySelectorAll(
                        ".report-column")
                    .forEach(x =>
                        x.checked = false);

                document
                    .querySelectorAll(
                        ".table-select-all")
                    .forEach(x =>
                        x.checked = false);

                updateSelectedCount();

            };

    }

}


/*
 * =====================================================
 * Search
 * =====================================================
 */

function filterColumns() {

    const query =
        document
            .getElementById("columnSearch")
            .value
            .trim()
            .toLowerCase();


    document
        .querySelectorAll(
            "#columnsContainer > div")
        .forEach(group => {

            const text =
                group.textContent
                    .toLowerCase();


            const matched =
                !query ||
                text.includes(query);


            group.classList.toggle(
                "hidden",
                !matched);

        });

}


/*
 * =====================================================
 * Selected Count
 * =====================================================
 */

export function updateSelectedCount() {

    const count =
        document.querySelectorAll(
            ".report-column:checked")
            .length;


    const element =
        document.getElementById(
            "selectedColumnCount");


    if (element)
        element.textContent =
            count;

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


            const selectedColumns = getSelectedColumns();


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
                            "Content-Type":
                                "application/json",

                            ...(token
                                ? {
                                    "RequestVerificationToken":
                                        token
                                }
                                : {})

                        },

                        body:
                            JSON.stringify(
                                model)

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
                .getElementById(
                    "reportEditorModal")
                .close();


            setTimeout(
                () =>
                    location.reload(),
                600);

        });


function getSelectedColumns() {

    return Array.from(
        document.querySelectorAll(
            ".report-column:checked"
        )
    ).map(x => ({
        table:
            x.dataset.table,

        column:
            x.dataset.column

    }));

}
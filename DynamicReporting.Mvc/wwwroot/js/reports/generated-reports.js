import {
    urls,
    activeFilters,
    sortColumn,
    sortDirection
} from "./state.js";

import {
    showNotification,
    escapeHtml
} from "./ui.js";

import {
    joinReportGroup
} from "./signalr.js";


export async function exportReport(id, type) {

    const params =
        new URLSearchParams();

    params.set(
        "reportDefinitionId",
        id);

    params.set(
        "type",
        type);


    if (activeFilters.length) {

        params.set(
            "filters",
            JSON.stringify(activeFilters));

    }


    if (sortColumn) {

        params.set(
            "sort",
            sortColumn);

        params.set(
            "dir",
            sortDirection);

    }


    const response =
        await fetch(
            `${urls.export}?${params.toString()}`);


    if (!response.ok) {

        showNotification(
            "شروع عملیات Export ناموفق بود.",
            "error");

        return;

    }


    const result =
        await response.json();


    showNotification(
        result.message ||
        `Export ${type.toUpperCase()} شروع شد. پس از آماده شدن اطلاع داده می‌شود.`,
        "info");


    if (result.reportId)
        await joinReportGroup(
            result.reportId);

}


/*
 * =====================================================
 * Generated Reports
 * =====================================================
 */

export async function openGeneratedReports() {

    document
        .getElementById("generatedReportsModal")
        .showModal();


    const container =
        document.getElementById(
            "generatedReportsContainer");


    container.innerHTML = `
        <div class="py-10 text-center">
            <span class="loading loading-spinner loading-lg"></span>
        </div>`;


    const response =
        await fetch(
            urls.generatedReports);


    if (!response.ok) {

        container.innerHTML = `
            <div class="alert alert-error">
                دریافت گزارش‌های تولید شده ناموفق بود.
            </div>`;

        return;
    }


    const reports =
        await response.json();


    if (!reports.length) {

        container.innerHTML = `
            <div class="py-10 text-center text-base-content/50">
                گزارشی تولید نشده است.
            </div>`;

        return;

    }


    /*
     * تمام وضعیت‌های موجود در گزارش‌ها
     * بدون تکرار
     */
    const statuses =
        [
            ...new Set(
                reports
                    .map(report => report.status)
                    .filter(status => status)
            )
        ];


    container.innerHTML = "";


    /*
     * =================================================
     * Status Filter
     * =================================================
     */

    const filterContainer =
        document.createElement("div");

    filterContainer.className =
        "mb-4 flex flex-wrap items-center gap-3";


    filterContainer.innerHTML = `
        <label
            for="generatedReportStatusFilter"
            class="font-semibold">

            وضعیت:

        </label>

        <select
            id="generatedReportStatusFilter"
            class="select select-bordered select-sm">

            <option value="">
                همه وضعیت‌ها
            </option>

            ${statuses
            .map(status => `
                    <option value="${escapeHtml(status)}">
                        ${escapeHtml(status)}
                    </option>
                `)
            .join("")}

        </select>
    `;


    container.appendChild(
        filterContainer);


    /*
     * Container مربوط به لیست گزارش‌ها
     */
    const reportsContainer =
        document.createElement("div");

    reportsContainer.id =
        "generatedReportsList";

    reportsContainer.className =
        "space-y-3";


    container.appendChild(
        reportsContainer);


    /*
     * نمایش اولیه تمام گزارش‌ها
     */
    renderGeneratedReports(
        reports,
        reportsContainer);


    /*
     * =================================================
     * Status Filter Change
     * =================================================
     */

    const statusFilter =
        document.getElementById(
            "generatedReportStatusFilter");


    statusFilter.addEventListener(
        "change",
        () => {

            const selectedStatus =
                statusFilter.value;


            const filteredReports =
                selectedStatus
                    ? reports.filter(
                        report =>
                            report.status ===
                            selectedStatus)
                    : reports;


            renderGeneratedReports(
                filteredReports,
                reportsContainer);

        });
}


/*
 * =====================================================
 * Render Generated Reports
 * =====================================================
 */

function renderGeneratedReports(
    reports,
    container) {

    container.innerHTML = "";


    if (!reports.length) {

        container.innerHTML = `
            <div class="py-10 text-center text-base-content/50">
                گزارشی با این وضعیت وجود ندارد.
            </div>`;

        return;
    }


    reports.forEach(report => {

        const item =
            document.createElement("div");


        item.className =
            "rounded-xl border border-base-300 bg-base-100 p-4";


        item.innerHTML = `

            <div class="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">

                <!-- Report Info -->
                <div class="min-w-0">

                    <!-- Status -->
                    <div class="flex flex-wrap items-center gap-2">

                        <span class="badge badge-primary">
                            ${escapeHtml(
            report.status ||
            "نامشخص")}
                        </span>

                        <span class="badge badge-ghost">
                            ${escapeHtml(
                report.fileType ||
                "نامشخص")}
                        </span>

                    </div>


                    <!-- Dates -->
                    <div class="mt-3 grid gap-2 text-sm text-base-content/60 sm:grid-cols-2">

                        <div class="flex items-center gap-2">

                            <span class="font-semibold text-base-content/80">
                                تاریخ تولید:
                            </span>

                            <span>
                                ${escapeHtml(
                    report.createAtDisplay)}
                            </span>

                        </div>


                        <div class="flex items-center gap-2">

                            <span class="font-semibold text-base-content/80">
                                تاریخ انقضا:
                            </span>

                            <span>
                                ${escapeHtml(
                        report.expDateTimeDisplay)}
                            </span>

                        </div>

                    </div>

                </div>


                <!-- Actions -->
                <div class="flex shrink-0 gap-2">

                    <a
                        href="${urls.downloadGeneratedReport}?id=${report.reportGuid}"
                        class="btn btn-sm btn-primary">

                        دانلود

                    </a>


                    <button
                        type="button"
                        class="btn btn-sm btn-error btn-outline"
                        onclick="deleteGeneratedReport('${report.reportGuid}')">

                        حذف

                    </button>

                </div>

            </div>

        `;


        container.appendChild(
            item);

    });
}


/*
 * =====================================================
 * Delete Generated Report
 * =====================================================
 */

export async function deleteGeneratedReport(id) {

    if (!confirm(
        "آیا از حذف این گزارش مطمئن هستید؟"))
        return;


    const response =
        await fetch(
            `${urls.deleteGeneratedReport}?id=${id}`,
            {
                method: "DELETE"
            });


    if (response.ok) {

        showNotification(
            "گزارش حذف شد.",
            "success");

        openGeneratedReports();

    }

}
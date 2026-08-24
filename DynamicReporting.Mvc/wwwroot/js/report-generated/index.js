import {
    showNotification,
    escapeHtml
} from "../reports/ui.js";


const urls =
    window.reportGeneratedPage;


let reports = [];

let filteredReports = [];

let currentPage = 1;

const pageSize = 10;


document.addEventListener(
    "DOMContentLoaded",
    loadGeneratedReports);


async function loadGeneratedReports() {

    const container =
        document.getElementById(
            "generatedReportsContainer");

    container.innerHTML = `
        <div class="py-10 text-center">
            <span class="loading loading-spinner loading-lg"></span>
        </div>`;

    try {

        const response =
            await fetch(
                urls.generatedReports);

        if (!response.ok)
            throw new Error();

        reports =
            await response.json();

        buildStatusFilter();

        filteredReports =
            [...reports];

        currentPage = 1;

        render();

    } catch (error) {
        console.error(error);
        container.innerHTML = `
            <div class="alert alert-error">
                دریافت گزارش‌های ایجاد شده ناموفق بود.
            </div>`;

    }
}


function buildStatusFilter() {

    const select =
        document.getElementById(
            "generatedReportStatusFilter");

    const statuses =
        [
            ...new Set(
                reports
                    .map(report => report.status)
                    .filter(Boolean)
            )
        ];

    select.innerHTML = `
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
    `;

    select.onchange = () => {

        const selectedStatus =
            select.value;

        filteredReports =
            selectedStatus
                ? reports.filter(
                    report =>
                        report.status ===
                        selectedStatus)
                : [...reports];

        currentPage = 1;

        render();
    };
}


function render() {

    renderReports();

    renderPagination();
}


function renderReports() {

    const container =
        document.getElementById(
            "generatedReportsContainer");

    const start =
        (currentPage - 1) * pageSize;

    const pageReports =
        filteredReports.slice(
            start,
            start + pageSize);

    container.innerHTML = "";

    if (!pageReports.length) {

        container.innerHTML = `
            <div class="py-10 text-center text-base-content/50">
                گزارشی برای نمایش وجود ندارد.
            </div>`;

        return;
    }

    pageReports.forEach(report => {

        const item =
            document.createElement("div");

        item.className =
            "rounded-xl border border-base-300 bg-base-100 p-4";

        item.innerHTML = `

            <div class="
                flex flex-col gap-4
                lg:flex-row
                lg:items-center
                lg:justify-between">

                <div class="min-w-0">

                    <div class="
                        flex flex-wrap
                        items-center gap-2">

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

                    <div class="
                        mt-3 grid gap-2
                        text-sm
                        text-base-content/60
                        sm:grid-cols-2">

                        <div>
                            <span class="font-semibold">
                                تاریخ تولید:
                            </span>

                            ${escapeHtml(
                    report.createAtDisplay)}
                        </div>

                        <div>
                            <span class="font-semibold">
                                تاریخ انقضا:
                            </span>

                            ${escapeHtml(
                        report.expDateTimeDisplay)}
                        </div>

                    </div>

                </div>

                <div class="flex shrink-0 gap-2">

                    <a
                        href="${urls.downloadGeneratedReport}?id=${encodeURIComponent(report.reportGuid)}"
                        class="btn btn-sm btn-primary">

                        دانلود

                    </a>

                    <button
                        type="button"
                        class="btn btn-sm btn-error btn-outline"
                        data-delete-id="${escapeHtml(report.reportGuid)}">

                        حذف

                    </button>

                </div>

            </div>
        `;

        item
            .querySelector("[data-delete-id]")
            .addEventListener(
                "click",
                () => deleteReport(
                    report.reportGuid));

        container.appendChild(item);
    });
}


function renderPagination() {

    const container =
        document.getElementById(
            "generatedReportsPagination");

    const totalPages =
        Math.ceil(
            filteredReports.length /
            pageSize);

    container.innerHTML = "";

    if (totalPages <= 1)
        return;

    const wrapper =
        document.createElement("div");

    wrapper.className = "join";

    for (
        let page = 1;
        page <= totalPages;
        page++
    ) {

        const button =
            document.createElement("button");

        button.className =
            `join-item btn ${page === currentPage
                ? "btn-active"
                : ""
            }`;

        button.textContent = page;

        button.onclick = () => {

            currentPage = page;

            render();

        };

        wrapper.appendChild(button);
    }

    container.appendChild(wrapper);
}


async function deleteReport(id) {

    if (!confirm(
        "آیا از حذف این گزارش مطمئن هستید؟"))
        return;

    const response =
        await fetch(
            `${urls.deleteGeneratedReport}?id=${encodeURIComponent(id)}`,
            {
                method: "DELETE"
            });

    if (!response.ok) {

        showNotification(
            "حذف گزارش ناموفق بود.",
            "error");

        return;
    }

    showNotification(
        "گزارش با موفقیت حذف شد.",
        "success");

    await loadGeneratedReports();
}
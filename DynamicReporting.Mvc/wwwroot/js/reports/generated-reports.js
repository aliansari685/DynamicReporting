import { urls } from "./state.js";
import { showNotification, escapeHtml } from "./ui.js";
import { joinReportGroup } from "./signalr.js";

export async function exportReport(id, type) {

    const response =
        await fetch(
            `${urls.export}?reportDefinitionId=${id}&type=${encodeURIComponent(type)}`
        );


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


    container.innerHTML = "";


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
                                            ${escapeHtml(report.status || "نامشخص")}
                                        </span>

                                        <span class="badge badge-ghost">
                                            ${escapeHtml(report.fileType || "نامشخص")}
                                        </span>

                                    </div>


                                    <!-- Dates -->
                                    <div class="mt-3 grid gap-2 text-sm text-base-content/60 sm:grid-cols-2">

                                        <div class="flex items-center gap-2">

                                            <span class="font-semibold text-base-content/80">
                                                تاریخ تولید:
                                            </span>

                                            <span>
                                                ${escapeHtml(report.createAtDisplay)}
                                            </span>

                                        </div>


                                        <div class="flex items-center gap-2">

                                            <span class="font-semibold text-base-content/80">
                                                تاریخ انقضا:
                                            </span>

                                            <span>
                                                ${escapeHtml(report.expDateTimeDisplay)}
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


        container.appendChild(item);

    });

}


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

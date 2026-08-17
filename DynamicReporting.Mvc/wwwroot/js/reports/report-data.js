import { selectedReportId, urls, currentPage, totalPages, setCurrentPage, setTotalPages } from "./state.js";
import { escapeHtml, showNotification } from "./ui.js";


export async function loadReportData(page = 1) {

    if (!selectedReportId)
        return;

    setCurrentPage(page);

    const response = await fetch(
        `${urls.data}?reportDefinitionId=${selectedReportId}&page=${page}&take=10`
    );

    if (!response.ok) {

        showNotification(
            "خطا در دریافت اطلاعات گزارش.",
            "error");

        return;
    }

    const result = await response.json();

    renderReportTable(result);

}


export function renderReportTable(result) {

    const head =
        document.getElementById("reportTableHead");

    const body =
        document.getElementById("reportTableBody");

    head.innerHTML = "";
    body.innerHTML = "";

    if (!result.data || result.data.length === 0) {

        body.innerHTML = `
                    <tr>
                        <td colspan="100%"
                            class="py-10 text-center">
                            داده‌ای برای نمایش وجود ندارد.
                        </td>
                    </tr>`;

        return;
    }

    const allColumns = Object.keys(result.data[0]);

    const half = Math.floor(allColumns.length / 2);

    // فقط ستون‌های فارسی
    const columns = allColumns.slice(half);

    // Header
    head.innerHTML = `
                <tr>
                    <th class="w-16 text-center">
                        ردیف
                    </th>

                    ${columns.map(x =>
        `<th>${escapeHtml(x)}</th>`
    ).join("")}
                </tr>`;

    // Rows
    result.data.forEach((row, index) => {

        const tr =
            document.createElement("tr");

        // شماره ردیف واقعی نسبت به کل صفحات
        const rowNumber =
            ((result.page - 1) * result.take) +
            index +
            1;

        const numberTd =
            document.createElement("td");

        numberTd.className =
            "text-center font-semibold text-base-content/60";

        numberTd.textContent =
            rowNumber;

        tr.appendChild(numberTd);

        // Data columns
        columns.forEach(column => {

            const td =
                document.createElement("td");

            td.textContent =
                row[column] ?? "";

            tr.appendChild(td);

        });

        body.appendChild(tr);

    });

    // Pagination
    setTotalPages(result.totalPages || 1);

    setCurrentPage(result.page || 1);

    document.getElementById("pageInfo").textContent =
        `صفحه ${currentPage} از ${totalPages}`;

    document.getElementById("previousPage").disabled =
        currentPage <= 1;

    document.getElementById("nextPage").disabled =
        currentPage >= totalPages;
}

export function changePage(delta) {

    const next =
        currentPage + delta;

    if (next < 1 || next > totalPages)
        return;

    loadReportData(next);

}

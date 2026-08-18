import {
    selectedReportId,
    urls,
    currentPage,
    totalPages,
    activeFilters,
    sortColumn,
    sortDirection,
    setCurrentPage,
    setTotalPages
} from "./state.js";

import {
    escapeHtml,
    showNotification
} from "./ui.js";

let isLoading = false;

function setGridLoading(loading) {

    isLoading = loading;

    const overlay =
        document.getElementById(
            "reportGridLoading");

    const previous =
        document.getElementById(
            "previousPage");

    const next =
        document.getElementById(
            "nextPage");

    const filterButton =
        document.getElementById(
            "openFilterPanel");


    if (overlay) {

        overlay.classList.toggle(
            "hidden",
            !loading);

        overlay.classList.toggle(
            "flex",
            loading);
    }


    if (previous)
        previous.disabled =
            loading ||
            currentPage <= 1;


    if (next)
        next.disabled =
            loading ||
            currentPage >= totalPages;


    if (filterButton)
        filterButton.disabled =
            loading;
}

function setReportStatus(text) {

    const status =
        document.getElementById(
            "reportStatus");

    if (status)
        status.textContent = text;
}

export async function loadReportData(page = 1) {

    if (!selectedReportId)
        return;


    // جلوگیری از درخواست همزمان
    if (isLoading)
        return;


    setCurrentPage(page);

    const params =
        // ReSharper disable once UseOfImplicitGlobalInFunctionScope
        // ReSharper disable once InconsistentNaming
        new URLSearchParams();


    params.set(
        "reportDefinitionId",
        selectedReportId);


    params.set(
        "page",
        page);


    params.set(
        "take",
        10);


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


    setGridLoading(true);

    setReportStatus(
        "در حال دریافت...");


    try {

        const response =
            await fetch(
                `${urls.data}?${params.toString()}`);


        if (!response.ok) {

            setReportStatus(
                "خطا");

            showNotification(
                "خطا در دریافت اطلاعات گزارش.",
                "error");

            return;
        }


        const result =
            await response.json();


        renderReportTable(result);


        setReportStatus(
            "آماده");
    } catch (error) {

        console.error(
            "خطا در دریافت اطلاعات گزارش:",
            error);

        setReportStatus(
            "خطا");

        showNotification(
            "خطا در دریافت اطلاعات گزارش.",
            "error");
    } finally {

        setGridLoading(false);
    }
}

export function renderReportTable(result) {

    const head =
        document.getElementById(
            "reportTableHead");

    const body =
        document.getElementById(
            "reportTableBody");

    if (!head || !body)
        return;

    head.innerHTML = "";
    body.innerHTML = "";

    if (!result.data ||
        result.data.length === 0) {

        body.innerHTML = `
            <tr>
                <td colspan="100%"
                    class="py-10 text-center">
                    داده‌ای برای نمایش وجود ندارد.
                </td>
            </tr>`;

        setTotalPages(
            result.totalPages || 1);

        updatePagination(
            result.page || 1);

        return;
    }

    const allColumns =
        Object.keys(
            result.data[0]);

    const half =
        Math.floor(
            allColumns.length / 2);

    const columns =
        allColumns.slice(half);

    head.innerHTML = `
        <tr>
            <th class="w-16 text-center">
                ردیف
            </th>

            ${columns.map(column =>
        `<th>${escapeHtml(column)}</th>`
    ).join("")}
        </tr>`;

    result.data.forEach(
        (row, index) => {

            const tr =
                document.createElement(
                    "tr");

            const rowNumber =
                ((result.page - 1) *
                    result.take) +
                index +
                1;

            const numberTd =
                document.createElement(
                    "td");

            numberTd.className =
                "text-center font-semibold text-base-content/60";

            numberTd.textContent =
                rowNumber;

            tr.appendChild(
                numberTd);

            columns.forEach(
                column => {

                    const td =
                        document.createElement(
                            "td");

                    td.textContent =
                        row[column] ?? "";

                    tr.appendChild(
                        td);

                });

            body.appendChild(
                tr);

        });

    setTotalPages(
        result.totalPages || 1);

    updatePagination(
        result.page || 1);
}

function updatePagination(page) {

    setCurrentPage(page);

    const pageInfo =
        document.getElementById(
            "pageInfo");

    if (pageInfo)
        pageInfo.textContent =
            `صفحه ${currentPage} از ${totalPages}`;

    const previous =
        document.getElementById(
            "previousPage");

    const next =
        document.getElementById(
            "nextPage");

    if (previous)
        previous.disabled =
            currentPage <= 1;

    if (next)
        next.disabled =
            currentPage >= totalPages;
}

export function changePage(delta) {

    if (isLoading)
        return;


    const next =
        currentPage + delta;


    if (next < 1 ||
        next > totalPages)
        return;


    loadReportData(next);
}
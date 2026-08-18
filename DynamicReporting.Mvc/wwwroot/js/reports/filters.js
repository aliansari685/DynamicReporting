import {
    selectedReportId,
    urls,
    activeFilters,
    sortColumn,
    sortDirection,
    addFilter,
    removeFilter,
    clearFilters,
    setSorting
} from "./state.js";

import {
    loadReportData
} from "./report-data.js";

import {
    showNotification
} from "./ui.js";


let filterableColumns = [];
let sortableColumns = [];


// ==================================================
// Load metadata
// ==================================================

export async function loadFilterMetadata() {

    if (!selectedReportId)
        return;

    try {

        await Promise.all([
            loadFilterableColumns(),
            loadSortableColumns()
        ]);

    } catch (error) {

        console.error(
            "خطا در دریافت اطلاعات فیلتر و مرتب‌سازی:",
            error);

        showNotification(
            "دریافت اطلاعات فیلتر و مرتب‌سازی ناموفق بود.",
            "error");
    }
}


// ==================================================
// Load filterable columns
// ==================================================

async function loadFilterableColumns() {

    const response =
        await fetch(
            `${urls.filterableColumns}?reportDefinitionId=${selectedReportId}`);

    if (!response.ok)
        throw new Error(
            "Filterable columns request failed.");

    const tables =
        await response.json();

    /*
     * API structure:
     *
     * [
     *   {
     *     tableName,
     *     tableDisplayName,
     *     columns: [
     *       {
     *         physicalName,
     *         displayName,
     *         supportedOperators: []
     *       }
     *     ]
     *   }
     * ]
     */

    filterableColumns =
        tables.flatMap(
            table =>
                (table.columns || []).map(
                    column => ({
                        tableName:
                            table.tableName,

                        tableDisplayName:
                            table.tableDisplayName,

                        field:
                            column.physicalName,

                        displayName:
                            column.displayName,

                        supportedOperators:
                            column.supportedOperators || []
                    })
                )
        );

    renderFilterColumns();
}


// ==================================================
// Load sortable columns
// ==================================================

async function loadSortableColumns() {

    const response =
        await fetch(
            `${urls.sortableColumns}?reportDefinitionId=${selectedReportId}`);

    if (!response.ok)
        throw new Error(
            "Sortable columns request failed.");

    sortableColumns =
        await response.json();

    renderSortColumns();
}


// ==================================================
// Render filter columns
// ==================================================

function renderFilterColumns() {

    const select =
        document.getElementById(
            "filterColumn");

    if (!select)
        return;

    select.innerHTML =
        `<option value="">
            انتخاب ستون
        </option>`;

    const groups = {};

    filterableColumns.forEach(
        column => {

            if (!groups[column.tableName]) {

                groups[column.tableName] = {
                    displayName:
                        column.tableDisplayName,

                    columns: []
                };
            }

            groups[column.tableName]
                .columns
                .push(column);
        });


    Object.values(groups).forEach(
        group => {

            const optgroup =
                document.createElement(
                    "optgroup");

            optgroup.label =
                group.displayName;

            group.columns.forEach(
                column => {

                    const option =
                        document.createElement(
                            "option");

                    option.value =
                        `${column.tableName}.${column.field}`;

                    option.textContent =
                        column.displayName;

                    optgroup.appendChild(
                        option);
                });

            select.appendChild(
                optgroup);
        });
}


// ==================================================
// Render sortable columns
// ==================================================

function renderSortColumns() {

    const select =
        document.getElementById(
            "sortColumn");

    if (!select)
        return;

    select.innerHTML =
        `<option value="">
            بدون مرتب‌سازی
        </option>`;

    sortableColumns.forEach(
        column => {

            const option =
                document.createElement(
                    "option");

            option.value =
                `${column.tableName}.${column.field}`;

            option.textContent =
                column.displayName ||
                column.field;

            select.appendChild(
                option);
        });
    select.value =
        sortColumn || "";
}


// ==================================================
// Open panel
// ==================================================

export function openFilterPanel() {

    const dialog =
        document.getElementById(
            "reportFilterPanel");

    if (!dialog)
        return;

    renderFilterRules();
    renderActiveFilterSummary();
    updateFilterCount();

    const sortSelect =
        document.getElementById(
            "sortColumn");

    const directionSelect =
        document.getElementById(
            "sortDirection");

    if (sortSelect)
        sortSelect.value =
            sortColumn || "";

    if (directionSelect)
        directionSelect.value =
            sortDirection || "asc";

    dialog.showModal();
}


// ==================================================
// Filter column changed
// ==================================================

function onFilterColumnChanged() {

    const columnSelect =
        document.getElementById(
            "filterColumn");

    const operatorSelect =
        document.getElementById(
            "filterOperator");

    const valueInput =
        document.getElementById(
            "filterValue");

    const addButton =
        document.getElementById(
            "addFilterRule");

    if (!columnSelect ||
        !operatorSelect ||
        !valueInput ||
        !addButton)
        return;


    const column =
        filterableColumns.find(
            item =>
                `${item.tableName}.${item.field}` === columnSelect.value);


    operatorSelect.innerHTML =
        `<option value="">
            انتخاب عملگر
        </option>`;

    valueInput.value = "";

    operatorSelect.disabled =
        !column;

    valueInput.disabled =
        !column;

    addButton.disabled =
        !column;


    if (!column)
        return;


    const operators =
        getOperatorsForColumn(
            column);


    operators.forEach(
        operator => {

            const option =
                document.createElement(
                    "option");

            option.value =
                operator.operator;

            option.textContent =
                operator.displayName;

            operatorSelect.appendChild(
                option);
        });


    updateValueInput();
}


// ==================================================
// Get operators from API
// ==================================================

function getOperatorsForColumn(column) {

    return column?.supportedOperators || [];
}


// ==================================================
// Operator changed
// ==================================================

function updateValueInput() {

    const operator =
        document.getElementById(
            "filterOperator");

    const input =
        document.getElementById(
            "filterValue");

    if (!operator || !input)
        return;


    if (
        operator.value === "isNull" ||
        operator.value === "isNotNull"
    ) {

        input.value = "";
        input.disabled = true;

        return;
    }


    input.disabled = false;
}


// ==================================================
// Add filter
// ==================================================

function addCurrentFilter() {

    const column =
        document.getElementById(
            "filterColumn");

    const operator =
        document.getElementById(
            "filterOperator");

    const value =
        document.getElementById(
            "filterValue");


    if (!column ||
        !operator ||
        !value)
        return;


    if (!column.value ||
        !operator.value)
        return;


    if (
        !value.disabled &&
        !value.value.trim()
    ) {

        showNotification(
            "مقدار فیلتر را وارد کنید.",
            "error");

        return;
    }


    addFilter({
        field:
            column.value,

        operator:
            operator.value,

        value:
            value.value.trim()
    });


    renderFilterRules();
    renderActiveFilterSummary();
    updateFilterCount();


    column.value = "";


    operator.innerHTML =
        `<option value="">
            ابتدا ستون را انتخاب کنید
        </option>`;

    operator.disabled = true;


    value.value = "";
    value.disabled = true;


    const addButton =
        document.getElementById(
            "addFilterRule");

    if (addButton)
        addButton.disabled = true;
}


// ==================================================
// Render active filter rules
// ==================================================

function renderFilterRules() {

    const container =
        document.getElementById(
            "filterRulesContainer");

    if (!container)
        return;


    container.innerHTML = "";


    if (!activeFilters.length) {

        container.innerHTML = `
            <div class="rounded-xl border border-dashed border-base-300 p-4 text-center text-sm text-base-content/50">
                هنوز فیلتری اضافه نشده است.
            </div>`;

        return;
    }


    activeFilters.forEach(
        (filter, index) => {

            const column =
                findColumn(
                    filter.field);


            const displayName =
                column?.displayName ||
                filter.field;


            const operatorName =
                getOperatorText(
                    filter.operator);


            const item =
                document.createElement(
                    "div");


            item.className =
                "flex items-center justify-between gap-3 rounded-xl border border-base-300 bg-base-100 p-3";


            item.innerHTML = `
                <div class="min-w-0">

                    <div class="font-semibold">
                        ${escapeHtml(displayName)}
                    </div>

                    <div class="mt-1 text-xs text-base-content/60">
                        ${escapeHtml(operatorName)}
                        ${filter.value
                    ? ` : ${escapeHtml(filter.value)}`
                    : ""
                }
                    </div>

                </div>

                <button
                    type="button"
                    class="btn btn-xs btn-ghost text-error"
                    data-remove-filter="${index}">
                    حذف
                </button>
            `;


            container.appendChild(
                item);
        });
}


// ==================================================
// Remove filter
// ==================================================

function removeFilterRule(index) {

    removeFilter(index);

    renderFilterRules();
    renderActiveFilterSummary();
    updateFilterCount();
}


// ==================================================
// Clear filters
// ==================================================

function clearAllFilters() {

    clearFilters();

    renderFilterRules();
    renderActiveFilterSummary();
    updateFilterCount();
}


// ==================================================
// Sorting
// ==================================================

function updateSorting() {

    const column =
        document.getElementById(
            "sortColumn");

    const direction =
        document.getElementById(
            "sortDirection");


    if (!column ||
        !direction)
        return;


    setSorting(
        column.value,
        direction.value);
}


// ==================================================
// Apply filter + sorting
// ==================================================

async function applyFilterPanel() {

    updateSorting();


    const dialog =
        document.getElementById(
            "reportFilterPanel");


    if (dialog)
        dialog.close();


    renderActiveFilterSummary();


    await loadReportData(1);
}


// ==================================================
// Reset panel
// ==================================================

function resetFilterPanel() {

    clearFilters();

    setSorting(
        "",
        "asc");


    renderFilterRules();
    renderActiveFilterSummary();
    updateFilterCount();


    const filterColumn =
        document.getElementById(
            "filterColumn");

    const filterOperator =
        document.getElementById(
            "filterOperator");

    const filterValue =
        document.getElementById(
            "filterValue");

    const sortColumnElement =
        document.getElementById(
            "sortColumn");

    const sortDirectionElement =
        document.getElementById(
            "sortDirection");


    if (filterColumn)
        filterColumn.value = "";


    if (filterOperator) {

        filterOperator.innerHTML =
            `<option value="">
                ابتدا ستون را انتخاب کنید
            </option>`;

        filterOperator.disabled = true;
    }


    if (filterValue) {

        filterValue.value = "";
        filterValue.disabled = true;
    }


    const addButton =
        document.getElementById(
            "addFilterRule");


    if (addButton)
        addButton.disabled = true;


    if (sortColumnElement)
        sortColumnElement.value = "";


    if (sortDirectionElement)
        sortDirectionElement.value = "asc";
}


// ==================================================
// Active filter summary
// ==================================================

export function renderActiveFilterSummary() {

    const container =
        document.getElementById(
            "activeFilterSummary");

    const badge =
        document.getElementById(
            "activeFilterCount");


    if (!container)
        return;


    container.innerHTML = "";


    if (badge) {

        badge.textContent =
            activeFilters.length;

        badge.classList.toggle(
            "hidden",
            activeFilters.length === 0);
    }


    activeFilters.forEach(
        filter => {

            const column =
                findColumn(
                    filter.field);


            const displayName =
                column?.displayName ||
                filter.field;


            const chip =
                document.createElement(
                    "span");


            chip.className =
                "badge badge-outline";


            chip.textContent =
                `${displayName}: ${filter.value}`;


            container.appendChild(
                chip);
        });
}


// ==================================================
// Filter count
// ==================================================

function updateFilterCount() {

    const count =
        document.getElementById(
            "panelFilterCount");


    if (count)
        count.textContent =
            activeFilters.length;
}


// ==================================================
// Find column
// ==================================================

function findColumn(field) {

    return filterableColumns.find(
        column =>
            `${column.tableName}.${column.field}` ===
            field);
}


// ==================================================
// Operator display name
// ==================================================

function getOperatorText(operator) {

    const column =
        filterableColumns.find(
            item =>
                item.supportedOperators?.some(
                    supported =>
                        supported.operator ===
                        operator
                )
        );


    const supported =
        column?.supportedOperators?.find(
            item =>
                item.operator ===
                operator
        );


    return supported?.displayName ||
        operator;
}


// ==================================================
// Escape HTML
// ==================================================

function escapeHtml(value) {

    return String(value ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}


// ==================================================
// Events
// ==================================================

document.addEventListener(
    "DOMContentLoaded",
    () => {

        const filterColumn =
            document.getElementById(
                "filterColumn");

        const filterOperator =
            document.getElementById(
                "filterOperator");

        const addButton =
            document.getElementById(
                "addFilterRule");

        const clearButton =
            document.getElementById(
                "clearAllFilters");

        const resetButton =
            document.getElementById(
                "resetFilterPanel");

        const applyButton =
            document.getElementById(
                "applyFilterPanel");

        const rulesContainer =
            document.getElementById(
                "filterRulesContainer");


        if (filterColumn)
            filterColumn.addEventListener(
                "change",
                onFilterColumnChanged);


        if (filterOperator)
            filterOperator.addEventListener(
                "change",
                updateValueInput);


        if (addButton)
            addButton.addEventListener(
                "click",
                addCurrentFilter);


        if (clearButton)
            clearButton.addEventListener(
                "click",
                clearAllFilters);


        if (resetButton)
            resetButton.addEventListener(
                "click",
                resetFilterPanel);


        if (applyButton)
            applyButton.addEventListener(
                "click",
                applyFilterPanel);


        if (rulesContainer)
            rulesContainer.addEventListener(
                "click",
                event => {

                    const button =
                        event.target.closest(
                            "[data-remove-filter]");


                    if (!button)
                        return;


                    const index =
                        Number(
                            button.dataset.removeFilter);


                    removeFilterRule(index);
                });
    });
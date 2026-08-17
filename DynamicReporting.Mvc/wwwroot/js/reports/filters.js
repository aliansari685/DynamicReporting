import { selectedReportId, urls } from "./state.js";

export async function loadFilterableColumns() {

    if (!selectedReportId)
        return;

    const response = await fetch(
        `${urls.filterableColumns}?reportDefinitionId=${selectedReportId}`
    );

    if (!response.ok)
        return;

    const columns =
        await response.json();

    const select =
        document.getElementById("filterColumn");

    if (!select)
        return;

    select.innerHTML =
        '<option value="">ستون فیلتر</option>';

    columns.forEach(column => {

        const option =
            document.createElement("option");

        option.value =
            column.field;

        option.textContent =
            column.displayName || column.field;

        select.appendChild(option);


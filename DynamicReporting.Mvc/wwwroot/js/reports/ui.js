
export function showNotification(
    message,
    type = "info") {

    const container =
        document.getElementById(
            "notificationContainer");


    const alert =
        document.createElement("div");


    alert.className =
        `alert alert-${type} shadow-lg`;


    alert.innerHTML = `
                        <span>
                            ${escapeHtml(message)}
                        </span>`;


    container.appendChild(alert);


    setTimeout(
        () => alert.remove(),
        5000);

}


/*
 * =====================================================
 * Helpers
 * =====================================================
 */

export function escapeHtml(value) {

    if (value === null ||
        value === undefined)
        return "";

    return String(value)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");

}


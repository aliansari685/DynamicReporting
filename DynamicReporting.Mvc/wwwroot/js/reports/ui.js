export function showNotification(
    message,
    type = "info") {

    const container =
        document.getElementById(
            "notificationContainer");

    if (!container)
        return;

    const notification =
        document.createElement("div");

    notification.className =
        `alert alert-${type} shadow-xl w-80
         transition-all duration-300`;

    notification.innerHTML = `
        <span>
            ${escapeHtml(message)}
        </span>

        <button
            type="button"
            class="btn btn-ghost btn-xs"
            aria-label="بستن">
            ✕
        </button>
    `;

    const closeButton =
        notification.querySelector("button");

    closeButton.addEventListener(
        "click",
        () => notification.remove());

    container.appendChild(notification);

    setTimeout(
        () => notification.remove(),
        5000);
}

export function showConfirm(
    message,
    title = "تأیید عملیات") {

    return new Promise(resolve => {

        const modal =
            document.getElementById(
                "confirmModal");

        const titleElement =
            document.getElementById(
                "confirmModalTitle");

        const messageElement =
            document.getElementById(
                "confirmModalMessage");

        const confirmButton =
            document.getElementById(
                "confirmModalConfirm");

        const cancelButton =
            document.getElementById(
                "confirmModalCancel");

        titleElement.textContent =
            title;

        messageElement.textContent =
            message;

        modal.showModal();

        const cleanup = result => {

            modal.close();

            confirmButton.onclick = null;
            cancelButton.onclick = null;

            resolve(result);
        };

        confirmButton.onclick =
            () => cleanup(true);

        cancelButton.onclick =
            () => cleanup(false);

    });
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
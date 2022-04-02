var modal;

window.showModal = (modalId) => {
    modal = new bootstrap.Modal(document.getElementById(modalId), {});
    modal.show();
};

window.hideModal = (modalId) => {
    modal.hide();
};
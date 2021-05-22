var modalBag = new ScapModalBag("modal-bag", "main[role='main']");

$.extend(true, $.fn.dataTable.defaults, {
    autoWidth: false,
    responsive: true,
    aLengthMenu: [5, 10, 25, 50],
    language: {
        url: "//cdn.datatables.net/plug-ins/1.10.22/i18n/Portuguese-Brasil.json",
        searchPlaceholder: "Buscar"
    },
    dom:
        "<'row'<'col-sm-12 col-md-6 order-2 order-md-1'f><'col-sm-12 col-md-6 order-1 order-md-2'l>>" +
        "<'row'<'col-sm-12't>>" +
        "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
});

$(document).ready(function () {
    $("#summary-msg-box").fadeOut(5000, "swing");
});
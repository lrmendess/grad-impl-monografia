class ScapModalBag
{
    constructor(id, parent) {
        $(parent).append("<div id='" + id + "'><div id='lll'></div></div>");

        this.id = id;
        this.attached = {};
    }

    get element() {
        return $("#" + this.id);
    }

    add(modalId, onLoad = null) {
        const modal = new ScapModal(modalId, onLoad);

        this.attached[modalId] = modal;
        this.element.append(modal.code);

        modal.trigger = $("a[data-modal='" + modalId + "']");
    }
}

class ScapModal
{
    constructor(id, onLoad = null) {
        this.id = id;
        this.onLoad = onLoad;
    }

    get element() {
        return $("#" + this.id);
    }

    get code() {
        return (
            '<div class="modal" tabindex="-1" role="dialog" id="' + this.id + '">' +
                '<div class="modal-dialog" role="document">' +
                    '<div class="modal-content">' +

                    '</div>' +
                '</div>' +
            '</div>'
        );
    }

    set content(html) {
        this.element.find('.modal-content').html(html);
    }

    load(href, action) {
        this.element.find('.modal-content').load(href, action);
    }

    show() {
        this.element.modal({ keyboard: true }, "show");
    }

    hide() {
        this.element.modal("hide");
    }

    set trigger(source) {
        const self = this;

        function ajaxAction(dialog, id, onLoad) {
            $.ajaxSetup({ cache: false });

            if (onLoad) onLoad();

            $("form", dialog).submit(function (e) {
                e.preventDefault();

                $('#lll').addClass('page-loading');

                const request = $.ajax({
                    url: this.action,
                    type: this.method,
                    data: new FormData(this),
                    processData: false,
                    contentType: false,
                });

                request.done(function (result) {
                    $('#lll').removeClass('page-loading');

                    if ($.type(result) == 'object') {
                        if (result.url) {
                            $(location).attr("href", result.url);
                        }

                        if (result.success) {
                            self.hide();
                        }

                        return false;
                    }

                    self.content = result;
                    ajaxAction(dialog, id, onLoad);
                });
                
                request.fail(function (result) {
                    alert(result);
                    $('#lll').removeClass('page-loading');
                    location.reload();
                });

                return false;
            });
        }

        $(source).click(function () {
            self.load(this.href, function () {
                self.show();
                ajaxAction(this, self.id, self.onLoad);
            });

            return false;
        });
    }
}
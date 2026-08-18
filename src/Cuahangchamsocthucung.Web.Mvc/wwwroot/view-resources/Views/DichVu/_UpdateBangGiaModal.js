
(function ($) {

    var _dichVuService = abp.services.app.dichvu,
        _$modal = $('#BangGiaEditModal'),
        _$form = _$modal.find('form');

    async function save() {

        if (!_$form.valid()) {
            return;
        }

        var bangGia = {
            Id: parseInt($('#bg-id').val(), 10),
            DichVuId: parseInt($('#bg-dichvu-id').val(), 10),
            Loaithucung: $('#bg-loaithucung').val(),
            Loailong: $('#bg-loailong').val() === 'true',
            Cannangtu: parseInt($('#bg-cannangtu').val(), 10),
            Cannangden: parseInt($('#bg-cannangden').val(), 10),
            Giadv: parseFloat($('#bg-giadv').val())
        };

        abp.ui.setBusy(_$form);

        try {

            await _dichVuService.updateBangGia(bangGia);

            abp.notify.success('Cập nhật thành công.');

            _$modal.modal('hide');

            abp.event.trigger('bangGia.edited', bangGia);

        } catch (error) {

            console.error(error);

            abp.notify.error(
                error.message || 'Có lỗi xảy ra khi cập nhật.'
            );

        } finally {

            abp.ui.clearBusy(_$form);
        }
    }

    // Nút Lưu
    _$form.closest('.modal-content')
        .find('.save-button')
        .click(function (e) {
            e.preventDefault();
            save();
        });

    // Nhấn Enter để lưu
    _$form.find('input').on('keypress', function (e) {

        if (e.which === 13) {
            e.preventDefault();
            save();
        }

    });

    // Focus khi mở modal
    _$modal.on('shown.bs.modal', function () {
        $('#bg-loaithucung').focus();
    });

})(jQuery);

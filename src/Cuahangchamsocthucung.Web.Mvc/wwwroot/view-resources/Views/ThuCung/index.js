(function ($) {
    var _thuCungService = abp.services.app.thuCung;
    var _$modal = $('#ThuCungCreateModal');
    var _$form = $('#thucung-create-form');
    var selectedFile = null;

    console.log('=== ThuCung index.js đã load ===');

    // ==================== THÊM THÚ CƯNG ====================
    _$form.on('submit', async function (e) {
        e.preventDefault();
        e.stopPropagation();

        var tenThuCung = $('#thucung-ten').val().trim();
        var loaiThuCung = $('#thucung-loai').val();
        var ghiChu = $('#thucung-ghichu').val().trim();

        if (!tenThuCung) {
            abp.notify.warn('Vui lòng nhập tên thú cưng.');
            $('#thucung-ten').focus();
            return false;
        }

        if (!loaiThuCung) {
            abp.notify.warn('Vui lòng chọn loại thú cưng.');
            $('#thucung-loai').focus();
            return false;
        }

        var input = {
            tenThuCung: tenThuCung,
            loaiThuCung: loaiThuCung,
            ghiChu: ghiChu,
            trangThai: true
        };

        abp.ui.setBusy(_$modal);

        try {
            var thuCungId = await _thuCungService.create(input);
            console.log('=== TẠO THÚ CƯNG THÀNH CÔNG ===', thuCungId);

            if (selectedFile) {
                var formData = new FormData();
                formData.append('thuCungId', thuCungId);
                formData.append('file', selectedFile);

                try {
                    await $.ajax({
                        url: '/api/services/app/ThuCung/UploadImage',
                        type: 'POST',
                        data: formData,
                        contentType: false,
                        processData: false
                    });
                    console.log('=== UPLOAD ẢNH THÀNH CÔNG ===');
                } catch (imageError) {
                    console.error('Upload ảnh lỗi:', imageError);
                    abp.notify.warn('Đã thêm thú cưng nhưng tải ảnh thất bại.');
                }
            }

            abp.notify.success('Thêm thú cưng thành công!');
            _$modal.modal('hide');

            setTimeout(function () {
                window.location.reload();
            }, 800);
        } catch (error) {
            console.error('=== CREATE THÚ CƯNG LỖI ===', error);
            abp.notify.error(getErrorMessage(error, 'Không thể thêm thú cưng.'));
        } finally {
            abp.ui.clearBusy(_$modal);
        }

        return false;
    });

    // ==================== CHỌN ẢNH THÊM ====================
    $('#thucung-image').on('change', function () {
        var file = this.files && this.files.length ? this.files[0] : null;
        selectedFile = null;
        $('#thucung-image-preview').empty();

        if (!file) return;

        var extension = file.name.split('.').pop().toLowerCase();
        var allowedExtensions = ['png', 'jpg', 'jpeg'];
        var allowedTypes = ['image/png', 'image/jpeg'];

        if (allowedExtensions.indexOf(extension) === -1 || allowedTypes.indexOf(file.type) === -1) {
            abp.notify.error('Chỉ chấp nhận 1 ảnh PNG, JPG hoặc JPEG.');
            $(this).val('');
            return;
        }

        selectedFile = file;

        var reader = new FileReader();
        reader.onload = function (e) {
            $('#thucung-image-preview').html(
                '<div class="position-relative d-inline-block">' +
                '<img src="' + e.target.result + '" class="img-thumbnail" style="width:180px;height:180px;object-fit:cover;">' +
                '<button type="button" id="btnRemoveThuCungImage" class="btn btn-danger btn-sm position-absolute" style="top:5px;right:5px;">×</button>' +
                '</div>' +
                '<div class="small text-muted mt-2">' + file.name + '</div>'
            );
        };
        reader.readAsDataURL(file);
    });

    $(document).on('click', '#btnRemoveThuCungImage', function () {
        $('#thucung-image').val('');
        $('#thucung-image-preview').empty();
        selectedFile = null;
    });

    _$modal.on('hidden.bs.modal', function () {
        if (_$form.length) _$form[0].reset();
        selectedFile = null;
        $('#thucung-image-preview').empty();
    });

    // ==================== ĐỔI TRẠNG THÁI ====================
    $(document).on('click', '.toggle-thucung', function () {
        var id = parseInt($(this).data('id'));
        var tenThuCung = $(this).data('name');
        var trangThaiHienTai = String($(this).data('status')).toLowerCase() === 'true';

        var hanhDong = trangThaiHienTai ? 'ngừng hoạt động' : 'kích hoạt';

        abp.message.confirm(
            'Bạn có chắc muốn ' + hanhDong + ' thú cưng "' + tenThuCung + '" không?',
            'Xác nhận',
            async function (isConfirmed) {
                if (!isConfirmed) return;

                abp.ui.setBusy();

                try {
                    await _thuCungService.changeStatus(id);

                    abp.notify.success(
                        trangThaiHienTai
                            ? 'Đã ngừng hoạt động thú cưng "' + tenThuCung + '".'
                            : 'Đã kích hoạt thú cưng "' + tenThuCung + '".'
                    );

                    setTimeout(function () {
                        window.location.reload();
                    }, 500);
                } catch (error) {
                    console.error('Lỗi đổi trạng thái thú cưng:', error);
                    abp.notify.error(
                        getErrorMessage(error, 'Không thể đổi trạng thái thú cưng.')
                    );
                } finally {
                    abp.ui.clearBusy();
                }
            }
        );
    });

    // ==================== SỬA THÚ CƯNG ====================
    var editSelectedFile = null;

    $(document).on('click', '.edit-thucung', async function () {
        var id = $(this).data('id');
        var _$editModal = $('#ThuCungEditModal');

        editSelectedFile = null;
        $('#edit-thucung-image').val('');
        $('#edit-thucung-image-preview').empty();

        abp.ui.setBusy(_$editModal);

        try {
            var data = await _thuCungService.get(id);

            $('#edit-thucung-id').val(data.id);
            $('#edit-thucung-ten').val(data.tenThuCung);
            $('#edit-thucung-loai').val(data.loaiThuCung);
            $('#edit-thucung-ghichu').val(data.ghiChu || '');

            if (data.imageUrl) {
                $('#edit-thucung-image-preview').html(
                    '<div class="position-relative d-inline-block">' +
                    '<img src="' + data.imageUrl + '" class="img-thumbnail" style="width:180px;height:180px;object-fit:cover;">' +
                    '<div class="small text-muted mt-2">Ảnh hiện tại</div>' +
                    '</div>'
                );
            }
        } catch (error) {
            console.error('Lỗi lấy thông tin thú cưng:', error);
            abp.notify.error(getErrorMessage(error, 'Không thể lấy thông tin thú cưng.'));
            _$editModal.modal('hide');
        } finally {
            abp.ui.clearBusy(_$editModal);
        }
    });

    $('#edit-thucung-image').on('change', function () {
        var file = this.files && this.files.length ? this.files[0] : null;
        editSelectedFile = null;

        if (!file) return;

        var extension = file.name.split('.').pop().toLowerCase();
        var allowedExtensions = ['png', 'jpg', 'jpeg'];
        var allowedTypes = ['image/png', 'image/jpeg'];

        if (allowedExtensions.indexOf(extension) === -1 || allowedTypes.indexOf(file.type) === -1) {
            abp.notify.error('Chỉ chấp nhận 1 ảnh PNG, JPG hoặc JPEG.');
            $(this).val('');
            return;
        }

        editSelectedFile = file;

        var reader = new FileReader();
        reader.onload = function (e) {
            $('#edit-thucung-image-preview').html(
                '<div class="position-relative d-inline-block">' +
                '<img src="' + e.target.result + '" class="img-thumbnail" style="width:180px;height:180px;object-fit:cover;">' +
                '<button type="button" id="btnRemoveEditThuCungImage" class="btn btn-danger btn-sm position-absolute" style="top:5px;right:5px;">×</button>' +
                '</div>' +
                '<div class="small text-muted mt-2">' + file.name + '</div>'
            );
        };
        reader.readAsDataURL(file);
    });

    $(document).on('click', '#btnRemoveEditThuCungImage', function () {
        $('#edit-thucung-image').val('');
        editSelectedFile = null;
        $('#edit-thucung-image-preview').empty();
    });

    $('#thucung-edit-form').on('submit', async function (e) {
        e.preventDefault();
        e.stopPropagation();

        var id = parseInt($('#edit-thucung-id').val());
        var tenThuCung = $('#edit-thucung-ten').val().trim();
        var loaiThuCung = $('#edit-thucung-loai').val();
        var ghiChu = $('#edit-thucung-ghichu').val().trim();
        var _$editModal = $('#ThuCungEditModal');

        if (!tenThuCung) {
            abp.notify.warn('Vui lòng nhập tên thú cưng.');
            $('#edit-thucung-ten').focus();
            return false;
        }

        if (!loaiThuCung) {
            abp.notify.warn('Vui lòng chọn loại thú cưng.');
            $('#edit-thucung-loai').focus();
            return false;
        }

        abp.ui.setBusy(_$editModal);

        try {
            await _thuCungService.update({
                id: id,
                tenThuCung: tenThuCung,
                loaiThuCung: loaiThuCung,
                ghiChu: ghiChu
            });

            if (editSelectedFile) {
                var formData = new FormData();
                formData.append('thuCungId', id);
                formData.append('file', editSelectedFile);

                try {
                    await $.ajax({
                        url: '/api/services/app/ThuCung/UploadImage',
                        type: 'POST',
                        data: formData,
                        contentType: false,
                        processData: false
                    });
                } catch (imageError) {
                    console.error('Upload ảnh sửa lỗi:', imageError);
                    abp.notify.warn('Thông tin đã được cập nhật nhưng thay ảnh thất bại.');
                }
            }

            abp.notify.success('Cập nhật thú cưng thành công!');
            _$editModal.modal('hide');

            setTimeout(function () {
                window.location.reload();
            }, 500);
        } catch (error) {
            console.error('=== UPDATE THÚ CƯNG LỖI ===', error);
            abp.notify.error(getErrorMessage(error, 'Không thể cập nhật thú cưng.'));
        } finally {
            abp.ui.clearBusy(_$editModal);
        }

        return false;
    });

    $('#ThuCungEditModal').on('hidden.bs.modal', function () {
        $('#thucung-edit-form')[0].reset();
        $('#edit-thucung-id').val('');
        $('#edit-thucung-image-preview').empty();
        editSelectedFile = null;
    });

    // ==================== XEM THÚ CƯNG ====================
    $(document).on('click', '.view-thucung', async function () {
        var id = parseInt($(this).data('id'));
        var _$viewModal = $('#ThuCungViewModal');

        if (!id) {
            abp.notify.error('Không xác định được thú cưng.');
            return;
        }

        abp.ui.setBusy(_$viewModal);

        try {
            // ABP proxy trả trực tiếp ThuCungDto
            var item = await _thuCungService.get(id);

            console.log('=== THÔNG TIN THÚ CƯNG ===', item);

            $('#view-thucung-ten').text(item.tenThuCung || '-');
            $('#view-thucung-loai').text(item.loaiThuCung || '-');
            $('#view-thucung-ghichu').text(item.ghiChu || 'Chưa có ghi chú.');

            var status = $('#view-thucung-trangthai');
            status
                .removeClass('bg-success bg-secondary')
                .addClass(item.trangThai ? 'bg-success' : 'bg-secondary')
                .text(item.trangThai ? 'Hoạt động' : 'Không hoạt động');

            if (item.imageUrl) {
                $('#view-thucung-image').html(
                    '<img src="' + item.imageUrl + '" class="img-fluid rounded-4 w-100" style="height:280px;object-fit:cover;" alt="' + (item.tenThuCung || '') + '">'
                );
            } else {
                var icon = item.loaiThuCung === 'Mèo' ? '🐱' : '🐶';

                $('#view-thucung-image').html(
                    '<div class="d-flex align-items-center justify-content-center bg-light rounded-4" style="height:280px;">' +
                    '<span class="display-1">' + icon + '</span>' +
                    '</div>'
                );
            }
        } catch (error) {
            console.error('Lỗi lấy thông tin thú cưng:', error);
            abp.notify.error(getErrorMessage(error, 'Không thể lấy thông tin thú cưng.'));
            _$viewModal.modal('hide');
        } finally {
            abp.ui.clearBusy(_$viewModal);
        }
    });

    // ==================== LỖI API ====================
    function getErrorMessage(error, defaultMessage) {
        if (error && error.message) return error.message;

        if (error &&
            error.responseJSON &&
            error.responseJSON.error &&
            error.responseJSON.error.message) {
            return error.responseJSON.error.message;
        }

        return defaultMessage;
    }
})(jQuery);
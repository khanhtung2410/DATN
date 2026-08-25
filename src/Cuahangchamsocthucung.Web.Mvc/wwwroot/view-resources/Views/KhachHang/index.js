(function ($) {
    var _khachHangService = abp.services.app.khachHang;
    var $table = $('#KhachHangTable');
    var $rows = $table.find('tbody .customer-row');
    var currentPage = 1;
    var pageSize = 10;
    var searchText = '';
    var sortColumn = null;
    var sortDirection = 'desc';

    $(function () {
        hienThiDanhSach();

        $('#customerSearch').on('input', function () {
            searchText = $(this).val().toLowerCase().trim();
            currentPage = 1;
            hienThiDanhSach();
        });

        $('#customerPageSize').on('change', function () {
            pageSize = parseInt($(this).val()) || 10;
            currentPage = 1;
            hienThiDanhSach();
        });

        $('.sortable').on('click', function () {
            var column = parseInt($(this).data('column'));

            if (sortColumn === column) {
                sortDirection = sortDirection === 'asc' ? 'desc' : 'asc';
            } else {
                sortColumn = column;
                sortDirection = 'desc';
            }

            $('.sortable i').removeClass('fa-sort-up fa-sort-down').addClass('fa-sort text-muted');

            var $icon = $(this).find('i');
            $icon.removeClass('fa-sort text-muted').addClass(sortDirection === 'asc' ? 'fa-sort-up' : 'fa-sort-down');

            currentPage = 1;
            hienThiDanhSach();
        });

        $(document).on('click', '.btn-chi-tiet', function (e) {
            e.preventDefault();

            var id = $(this).data('id');
            var $modal = $('#KhachHangDetailModal');

            resetModal();
            $modal.modal('show');
            abp.ui.setBusy($modal);

            _khachHangService.getKhachHangById(id)
                .done(function (result) {
                    hienThiChiTiet(result.result || result);
                })
                .fail(function (error) {
                    console.error('Lỗi lấy chi tiết khách hàng:', error);
                    console.error('Response:', error.responseJSON);
                    abp.message.error(error.responseJSON?.error?.message || 'Không thể tải thông tin khách hàng.');
                })
                .always(function () {
                    abp.ui.clearBusy($modal);
                });
        });
        $(document).on('click', '.toggle-status', function (e) {
            e.preventDefault();

            var id = $(this).data('id');
            var $button = $(this);

            abp.message.confirm(
                'Bạn có chắc muốn thay đổi trạng thái khách hàng này?',
                'Xác nhận',
                function (isConfirmed) {
                    if (!isConfirmed) {
                        return;
                    }

                    abp.ui.setBusy($button);

                    _khachHangService.thayDoiTrangThai(id)
                        .done(function () {
                            abp.notify.success('Đã thay đổi trạng thái khách hàng.');
                            location.reload();
                        })
                        .fail(function (error) {
                            console.error('Lỗi thay đổi trạng thái:', error);
                            abp.message.error(
                                error?.responseJSON?.error?.message || 'Không thể thay đổi trạng thái khách hàng.'
                            );
                        })
                        .always(function () {
                            abp.ui.clearBusy($button);
                        });
                }
            );
        });
    });

    function hienThiDanhSach() {
        var filtered = [];

        $rows.each(function () {
            var $row = $(this);
            var text = $row.text().toLowerCase();

            if (!searchText || text.indexOf(searchText) !== -1) {
                filtered.push($row);
            }
        });

        if (sortColumn !== null) {
            filtered.sort(function (a, b) {
                var valueA = sortColumn === 3 ? Number(a.data('vip')) || 0 : Number(a.data('total')) || 0;
                var valueB = sortColumn === 3 ? Number(b.data('vip')) || 0 : Number(b.data('total')) || 0;

                return sortDirection === 'asc' ? valueA - valueB : valueB - valueA;
            });
        }

        $rows.hide();

        var total = filtered.length;
        var totalPages = Math.ceil(total / pageSize) || 1;

        if (currentPage > totalPages) {
            currentPage = totalPages;
        }

        var start = (currentPage - 1) * pageSize;
        var end = Math.min(start + pageSize, total);

        for (var i = start; i < end; i++) {
            filtered[i].show();
        }

        if (total === 0) {
            if (!$table.find('tbody .empty-row').length) {
                $table.find('tbody').append('<tr class="empty-row"><td colspan="6" class="text-center text-muted py-4">Không tìm thấy khách hàng</td></tr>');
            }
        } else {
            $table.find('tbody .empty-row').remove();
        }

        $('#customerInfo').text(
            total === 0
                ? 'Không có khách hàng'
                : 'Hiển thị ' + (start + 1) + ' đến ' + end + ' trong tổng số ' + total + ' khách hàng'
        );

        hienThiPhanTrang(totalPages);
    }

    function hienThiPhanTrang(totalPages) {
        var $pagination = $('#customerPagination');
        $pagination.empty();

        if (totalPages <= 1) {
            return;
        }

        var html = '<ul class="pagination mb-0">';

        html += '<li class="page-item ' + (currentPage === 1 ? 'disabled' : '') + '">';
        html += '<a class="page-link customer-page" href="#" data-page="' + (currentPage - 1) + '">Trước</a>';
        html += '</li>';

        for (var i = 1; i <= totalPages; i++) {
            if (i === 1 || i === totalPages || Math.abs(i - currentPage) <= 1) {
                html += '<li class="page-item ' + (i === currentPage ? 'active' : '') + '">';
                html += '<a class="page-link customer-page" href="#" data-page="' + i + '">' + i + '</a>';
                html += '</li>';
            } else if (i === currentPage - 2 || i === currentPage + 2) {
                html += '<li class="page-item disabled"><span class="page-link">...</span></li>';
            }
        }

        html += '<li class="page-item ' + (currentPage === totalPages ? 'disabled' : '') + '">';
        html += '<a class="page-link customer-page" href="#" data-page="' + (currentPage + 1) + '">Sau</a>';
        html += '</li>';

        html += '</ul>';

        $pagination.html(html);

        $('.customer-page').on('click', function (e) {
            e.preventDefault();

            var page = parseInt($(this).data('page'));

            if (page >= 1 && page <= totalPages && page !== currentPage) {
                currentPage = page;
                hienThiDanhSach();
            }
        });
    }

    function resetModal() {
        $('#detailHoTen').text('Đang tải...');
        $('#detailSDT').text('-');
        $('#detailEmail').text('-');
        $('#detailTrangThai').html('-');
        $('#detailVip').html('-');
        $('#detailTongChi').text('-');
        $('#detailVipTiepTheo').html('-');
        $('#detailMucChi').text('-');
        $('#detailConThieu').html('-');
        $('#detailThuCungs').html('<div class="text-muted">Đang tải...</div>');
    }

    function hienThiChiTiet(data) {
        var capVip = Number(data.capVip) || 0;
        var tongChiTieu = Number(data.tongChiTieu) || 0;
        var mucChiTieu = Number(data.mucChiTieuVipTiepTheo) || 0;
        var conThieu = Number(data.conThieuVip) || 0;

        $('#detailHoTen').text(data.hoten || '-');
        $('#detailSDT').text(data.sdt || '-');
        $('#detailEmail').text(data.email || '-');
        $('#detailTrangThai').html(data.trangThai
            ? '<span class="badge badge-success">Hoạt động</span>'
            : '<span class="badge badge-secondary">Không hoạt động</span>');

        $('#detailVip').html(capVip > 0
            ? '<span class="badge badge-warning">' + escapeHtml(data.tenVip || ('VIP ' + capVip)) + '</span>'
            : '<span class="badge badge-secondary">Thường</span>');

        $('#detailTongChi').text(tien(tongChiTieu));

        if (data.tenVipTiepTheo) {
            $('#detailVipTiepTheo').html('<span class="badge badge-info">' + escapeHtml(data.tenVipTiepTheo) + '</span>');
            $('#detailMucChi').text(tien(mucChiTieu));
            $('#detailConThieu').text(tien(conThieu));
        } else {
            $('#detailVipTiepTheo').html('<span class="text-success">Đã đạt cấp cao nhất</span>');
            $('#detailMucChi').text('-');
            $('#detailConThieu').html('<span class="text-success">0 đ</span>');
        }

        hienThiThuCungs(data.thuCungs || []);
    }

    function hienThiThuCungs(thuCungs) {
        if (!thuCungs.length) {
            $('#detailThuCungs').html('<div class="alert alert-light mb-0">Khách hàng chưa có thú cưng.</div>');
            return;
        }

        var html = '<div class="row">';

        $.each(thuCungs, function (i, tc) {
            var image = tc.imageUrl
                ? '<img src="' + escapeAttribute(tc.imageUrl) + '" class="img-thumbnail" style="width:90px;height:90px;object-fit:cover;">'
                : '<div class="bg-light border rounded d-flex align-items-center justify-content-center" style="width:90px;height:90px;"><i class="fa fa-paw fa-2x text-muted"></i></div>';

            html += '<div class="col-md-6 mb-3">';
            html += '<div class="border rounded p-2 h-100">';
            html += '<div class="d-flex">';
            html += '<div class="mr-3">' + image + '</div>';
            html += '<div>';
            html += '<strong>' + escapeHtml(tc.tenThuCung || '-') + '</strong><br>';
            html += '<span>' + escapeHtml(tc.loaiThuCung || '-') + '</span><br>';
            html += '<small class="text-muted">' + escapeHtml(tc.ghiChu || 'Không có ghi chú') + '</small><br>';
            html += tc.trangThai
                ? '<span class="badge badge-success mt-1">Hoạt động</span>'
                : '<span class="badge badge-secondary mt-1">Không hoạt động</span>';
            html += '</div></div></div></div>';
        });

        html += '</div>';
        $('#detailThuCungs').html(html);
    }

    function tien(value) {
        return value.toLocaleString('vi-VN') + ' đ';
    }

    function escapeHtml(value) {
        return $('<div>').text(value).html();
    }

    function escapeAttribute(value) {
        return $('<div>').text(value).html().replace(/"/g, '&quot;');
    }
})(jQuery);
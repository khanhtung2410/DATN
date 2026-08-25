(function () {

    var doanhThuChart = null;
    var dichVuChart = null;

    // =========================
    // FORMAT TIỀN
    // =========================

    function formatTien(value) {

        return Number(value || 0)
            .toLocaleString('vi-VN') + ' ₫';

    }

    // =========================
    // KHỞI TẠO NĂM
    // =========================

    function khoiTaoNam() {

        var namHienTai = moment().year();

        var html = '';

        // Cho phép chọn 5 năm trước
        // đến 2 năm sau

        for (var nam = namHienTai - 5;
            nam <= namHienTai + 2;
            nam++) {

            html += '<option value="' +
                nam +
                '">' +
                nam +
                '</option>';
        }

        $('#nam').html(html);

        $('#nam').val(namHienTai);
    }

    // =========================
    // KHỞI TẠO THÁNG
    // =========================

    function khoiTaoThang() {

        var thangHienTai =
            moment().month() + 1;

        $('#thang').val(thangHienTai);
    }

    // =========================
    // LOAD BÁO CÁO
    // =========================

    function loadBaoCao() {

        var thang =
            parseInt($('#thang').val());

        var nam =
            parseInt($('#nam').val());

        if (!thang || thang < 1 || thang > 12) {

            abp.notify.warn(
                'Vui lòng chọn tháng.'
            );

            return;
        }

        if (!nam) {

            abp.notify.warn(
                'Vui lòng chọn năm.'
            );

            return;
        }

        abp.ui.setBusy($('.content'));

        abp.services.app.baoCao
            .getBaoCao({

                thang: thang,

                nam: nam

            })
            .done(function (result) {

                hienThiBaoCao(result);

            })
            .fail(function (error) {

                abp.notify.error(
                    error.message ||
                    'Không thể tải báo cáo.'
                );

            })
            .always(function () {

                abp.ui.clearBusy($('.content'));

            });
    }

    // =========================
    // HIỂN THỊ BÁO CÁO
    // =========================

    function hienThiBaoCao(result) {

        result = result || {};

        var doanhThuDaThanhToan =
            Number(
                result.doanhThuDaThanhToan || 0
            );

        var tongDoanhThu =
            Number(
                result.tongDoanhThu || 0
            );

        var chiPhiLuong =
            Number(
                result.tongChiPhiLuong || 0
            );

        var loiNhuan =
            Number(
                result.loiNhuan || 0
            );

        var doanhThuChuaThanhToan =
            Math.max(
                0,
                tongDoanhThu -
                doanhThuDaThanhToan
            );

        var doanhThuTheoNgay =
            result.doanhThuTheoNgay || [];

        var doanhThuTheoDichVu =
            result.doanhThuTheoDichVu || [];

        // =========================
        // KPI
        // =========================

        $('#doanhThu')
            .text(
                formatTien(
                    doanhThuDaThanhToan
                )
            );

        $('#chiPhiLuong')
            .text(
                formatTien(
                    chiPhiLuong
                )
            );

        $('#loiNhuan')
            .text(
                formatTien(
                    loiNhuan
                )
            );

        $('#doanhThuChuaThanhToan')
            .text(
                formatTien(
                    doanhThuChuaThanhToan
                )
            );

        // =========================
        // THỐNG KÊ
        // =========================

        $('#tongLichChamSoc')
            .text(
                result.tongLichChamSoc || 0
            );

        $('#tongKhachHang')
            .text(
                result.tongKhachHang || 0
            );

        $('#tongThuCung')
            .text(
                result.tongThuCung || 0
            );

        $('#lichHoanThanh')
            .text(
                result.lichHoanThanh || 0
            );

        $('#lichDangDienRa')
            .text(
                result.lichDangDienRa || 0
            );

        $('#lichChoXacNhan')
            .text(
                result.lichChoXacNhan || 0
            );

        $('#lichDaXacNhan')
            .text(
                result.lichDaXacNhan || 0
            );

        $('#lichDaHuy')
            .text(
                result.lichDaHuy || 0
            );

        $('#lichBiTuChoi')
            .text(
                result.lichBiTuChoi || 0
            );

        // =========================
        // BIỂU ĐỒ
        // =========================

        veBieuDoDoanhThu(
            doanhThuTheoNgay
        );

        veBieuDoDichVu(
            doanhThuTheoDichVu
        );

        // =========================
        // BẢNG
        // =========================

        hienThiBangDichVu(
            doanhThuTheoDichVu
        );
    }

    // =========================
    // BIỂU ĐỒ DOANH THU
    // =========================

    function veBieuDoDoanhThu(data) {

        var labels =
            data.map(function (x) {

                return moment(x.ngay)
                    .format('DD/MM');

            });

        var values =
            data.map(function (x) {

                return Number(
                    x.doanhThu || 0
                );

            });

        if (doanhThuChart) {

            doanhThuChart.destroy();

        }

        doanhThuChart = new Chart(
            $('#doanhThuChart'),
            {
                type: 'line',

                data: {

                    labels: labels,

                    datasets: [{

                        label: 'Doanh thu',

                        data: values,

                        borderColor: '#007bff',

                        backgroundColor:
                            'rgba(0, 123, 255, 0.12)',

                        pointBackgroundColor:
                            '#007bff',

                        pointBorderColor:
                            '#fff',

                        pointBorderWidth: 2,

                        pointRadius: 4,

                        pointHoverRadius: 7,

                        borderWidth: 3,

                        fill: true,

                        lineTension: 0.35

                    }]

                },

                options: {

                    responsive: true,

                    maintainAspectRatio: false,

                    legend: {

                        display: false

                    },

                    tooltips: {

                        displayColors: false,

                        backgroundColor:
                            'rgba(0,0,0,0.8)',

                        titleFontSize: 13,

                        bodyFontSize: 13,

                        xPadding: 12,

                        yPadding: 10,

                        callbacks: {

                            title: function (
                                tooltipItems
                            ) {

                                return 'Ngày ' +
                                    tooltipItems[0]
                                        .label;

                            },

                            label: function (
                                tooltipItem
                            ) {

                                return 'Doanh thu: ' +
                                    formatTien(
                                        tooltipItem.yLabel
                                    );

                            }

                        }

                    },

                    scales: {

                        xAxes: [{

                            gridLines: {

                                display: false

                            },

                            ticks: {

                                autoSkip: true,

                                maxTicksLimit: 15,

                                fontSize: 12

                            }

                        }],

                        yAxes: [{

                            gridLines: {

                                color:
                                    'rgba(0,0,0,0.06)',

                                drawBorder: false

                            },

                            ticks: {

                                beginAtZero: true,

                                padding: 10,

                                callback: function (
                                    value
                                ) {

                                    if (
                                        value >=
                                        1000000
                                    ) {

                                        return (
                                            value /
                                            1000000
                                        ).toFixed(
                                            value %
                                                1000000 ===
                                                0
                                                ? 0
                                                : 1
                                        ) + ' Tr';

                                    }

                                    if (
                                        value >=
                                        1000
                                    ) {

                                        return (
                                            value /
                                            1000
                                        ).toFixed(0)
                                            + 'K';

                                    }

                                    return value;

                                }

                            }

                        }]

                    },

                    elements: {

                        line: {

                            tension: 0.35

                        }

                    }

                }

            }
        );
    }

    // =========================
    // BIỂU ĐỒ DỊCH VỤ
    // =========================

    function veBieuDoDichVu(data) {

        var labels =
            data.map(function (x) {

                return x.tenDichVu;

            });

        var values =
            data.map(function (x) {

                return Number(
                    x.doanhThu || 0
                );

            });

        var colors = [

            '#007bff',
            '#28a745',
            '#ffc107',
            '#dc3545',
            '#17a2b8',
            '#6f42c1',
            '#fd7e14',
            '#20c997'

        ];

        if (dichVuChart) {

            dichVuChart.destroy();

        }

        dichVuChart = new Chart(
            $('#dichVuChart'),
            {
                type: 'doughnut',

                data: {

                    labels: labels,

                    datasets: [{

                        data: values,

                        backgroundColor:
                            colors.slice(
                                0,
                                values.length
                            ),

                        borderColor: '#fff',

                        borderWidth: 3,

                        hoverBorderColor: '#fff',

                        hoverBorderWidth: 4

                    }]

                },

                options: {

                    responsive: true,

                    maintainAspectRatio: false,

                    cutoutPercentage: 62,

                    legend: {

                        position: 'right',

                        labels: {

                            padding: 15,

                            boxWidth: 18,

                            fontSize: 13

                        }

                    },

                    tooltips: {

                        backgroundColor:
                            'rgba(0,0,0,0.8)',

                        displayColors: true,

                        callbacks: {

                            label: function (
                                tooltipItem,
                                data
                            ) {

                                var index =
                                    tooltipItem.index;

                                return data.labels[index] +
                                    ': ' +
                                    formatTien(
                                        data.datasets[0]
                                            .data[index]
                                    );

                            }

                        }

                    }

                }

            }
        );
    }

    // =========================
    // HTML ENCODE
    // =========================

    function htmlEncode(value) {

        return $('<div>')
            .text(value || '')
            .html();

    }

    // =========================
    // BẢNG DỊCH VỤ
    // =========================

    function hienThiBangDichVu(data) {

        var html = '';

        var tongSoLuong = 0;

        var tongDoanhThu = 0;

        if (!data.length) {

            html =
                '<tr>' +
                '<td colspan="4" ' +
                'class="text-center text-muted py-4">' +
                'Chưa có dữ liệu doanh thu dịch vụ.' +
                '</td>' +
                '</tr>';

        }
        else {

            data.forEach(function (
                item,
                index
            ) {

                var soLuong =
                    Number(
                        item.soLuong || 0
                    );

                var doanhThu =
                    Number(
                        item.doanhThu || 0
                    );

                tongSoLuong +=
                    soLuong;

                tongDoanhThu +=
                    doanhThu;

                html +=

                    '<tr>' +

                    '<td class="text-center text-muted">' +
                    (index + 1) +
                    '</td>' +

                    '<td>' +
                    '<strong>' +
                    htmlEncode(
                        item.tenDichVu ||
                        'Không xác định'
                    ) +
                    '</strong>' +
                    '</td>' +

                    '<td class="text-center">' +

                    '<span class="badge badge-light px-3 py-2">' +
                    soLuong +
                    '</span>' +

                    '</td>' +

                    '<td class="text-right font-weight-bold">' +
                    formatTien(
                        doanhThu
                    ) +
                    '</td>' +

                    '</tr>';

            });

        }

        $('#baoCaoDichVuBody')
            .html(html);

        $('#tongSoLuongDichVu')
            .text(tongSoLuong);

        $('#tongDoanhThuDichVu')
            .text(
                formatTien(
                    tongDoanhThu
                )
            );
    }

    // =========================
    // SỰ KIỆN
    // =========================

    $('#btnBaoCao').click(function () {

        loadBaoCao();

    });

    // Cho phép Enter
    // sau khi chọn tháng/năm

    $('#thang, #nam').change(function () {

        loadBaoCao();

    });

    // =========================
    // KHỞI TẠO
    // =========================

    $(function () {

        khoiTaoNam();

        khoiTaoThang();

        loadBaoCao();

    });

})();
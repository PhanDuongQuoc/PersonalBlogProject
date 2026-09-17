import pptxgen from "pptxgenjs";
import type {
  AdminAnalyticsSummary,
  CategoryAnalyticsItem,
  MonthlyCommentsItem,
  MonthlyContactsItem,
  ViewsTrendPoint
} from "@/types/admin-analytics";

const BRAND_ROSE = "DF266A";
const BRAND_DARK = "0B1326";
const BRAND_MUTED = "64748B";
const BRAND_INDIGO = "4F46E5";
const BRAND_EMERALD = "10B981";
const BRAND_AMBER = "F59E0B";

/**
 * 1. Xuất toàn bộ Báo cáo Thống kê Tổng quan (Full Report PPTX)
 */
export async function exportFullAnalyticsReportPptx(summary: AdminAnalyticsSummary) {
  const pptx = new pptxgen();
  pptx.layout = "LAYOUT_16x9";
  pptx.author = "PDQ Blog Admin";
  pptx.company = "PDQ Portfolio System";
  pptx.title = "Báo cáo Thống kê Hoạt động Blog";

  const dateStr = new Date().toLocaleDateString("vi-VN");

  // === SLIDE 1: Cover & KPI Summary ===
  const slide1 = pptx.addSlide();
  slide1.background = { color: "F8FAFC" };

  // Header Title
  slide1.addText("BÁO CÁO THỐNG KÊ HOẠT ĐỘNG BLOG", {
    x: 0.8,
    y: 0.6,
    w: 8.4,
    h: 0.6,
    fontSize: 22,
    fontFace: "Arial",
    bold: true,
    color: BRAND_ROSE
  });

  slide1.addText(`Tổng hợp chỉ số hiệu suất & lưu lượng độc giả · Ngày xuất: ${dateStr}`, {
    x: 0.8,
    y: 1.15,
    w: 8.4,
    h: 0.35,
    fontSize: 12,
    fontFace: "Arial",
    color: BRAND_MUTED
  });

  // 4 KPI Stat Boxes
  const kpis = [
    {
      title: "TỔNG LƯỢT XEM",
      val: `${summary.totalViews.toLocaleString()} (+${summary.viewsGrowthRate}%)`,
      sub: "Lưu lượng độc giả toàn hệ thống",
      color: BRAND_ROSE,
      x: 0.8,
      y: 1.7
    },
    {
      title: "TỔNG BÀI VIẾT",
      val: `${summary.totalPosts} bài`,
      sub: `${summary.publishedPosts} đã xuất bản · ${summary.draftPosts} nháp`,
      color: BRAND_INDIGO,
      x: 5.2,
      y: 1.7
    },
    {
      title: "BÌNH LUẬN & PHẢN HỒI",
      val: `${summary.totalComments} phản hồi`,
      sub: `${summary.approvedComments} đã duyệt · ${summary.pendingComments} chờ`,
      color: BRAND_EMERALD,
      x: 0.8,
      y: 3.3
    },
    {
      title: "DANH MỤC & THẺ",
      val: `${summary.totalCategories} Danh mục`,
      sub: `${summary.totalTags} Thẻ gắn phân loại`,
      color: BRAND_AMBER,
      x: 5.2,
      y: 3.3
    }
  ];

  kpis.forEach((k) => {
    slide1.addShape(pptx.ShapeType.roundRect, {
      x: k.x,
      y: k.y,
      w: 4.0,
      h: 1.35,
      rectRadius: 0.1,
      fill: { color: "FFFFFF" },
      line: { color: "E2E8F0", width: 1 }
    });

    slide1.addText(k.title, {
      x: k.x + 0.2,
      y: k.y + 0.15,
      w: 3.6,
      h: 0.25,
      fontSize: 10,
      fontFace: "Arial",
      bold: true,
      color: k.color
    });

    slide1.addText(k.val, {
      x: k.x + 0.2,
      y: k.y + 0.4,
      w: 3.6,
      h: 0.45,
      fontSize: 18,
      fontFace: "Arial",
      bold: true,
      color: BRAND_DARK
    });

    slide1.addText(k.sub, {
      x: k.x + 0.2,
      y: k.y + 0.85,
      w: 3.6,
      h: 0.3,
      fontSize: 10,
      fontFace: "Arial",
      color: BRAND_MUTED
    });
  });

  // Footer Slide 1
  slide1.addText("PDQ Portfolio Blog System · Slide 1", {
    x: 0.8,
    y: 5.0,
    w: 8.4,
    h: 0.3,
    fontSize: 9,
    fontFace: "Arial",
    color: "94A3B8",
    align: "right"
  });

  // === SLIDE 2: Detailed KPI Table & Status Breakdown ===
  const slide2 = pptx.addSlide();
  slide2.background = { color: "F8FAFC" };

  slide2.addText("CHI TIẾT CHỈ SỐ HOẠT ĐỘNG & CƠ CẤU HỆ THỐNG", {
    x: 0.8,
    y: 0.6,
    w: 8.4,
    h: 0.5,
    fontSize: 18,
    fontFace: "Arial",
    bold: true,
    color: BRAND_ROSE
  });

  const tableRows: pptxgen.TableRow[] = [
    [
      { text: "Hạng mục", options: { bold: true, fill: { color: "F1F5F9" }, color: BRAND_DARK } },
      { text: "Số lượng / Tỷ lệ", options: { bold: true, fill: { color: "F1F5F9" }, color: BRAND_DARK } },
      { text: "Mô tả chi tiết", options: { bold: true, fill: { color: "F1F5F9" }, color: BRAND_DARK } }
    ],
    [
      { text: "Tổng lượt xem Blog" },
      { text: `${summary.totalViews.toLocaleString()} lượt`, options: { bold: true, color: BRAND_ROSE } },
      { text: `Lưu lượng độc giả truy cập toàn hệ thống (+${summary.viewsGrowthRate}% tăng trưởng)` }
    ],
    [
      { text: "Bài viết đã xuất bản" },
      { text: `${summary.publishedPosts} bài`, options: { bold: true, color: BRAND_EMERALD } },
      { text: "Các bài viết đang công khai cho độc giả theo dõi" }
    ],
    [
      { text: "Bản nháp & Lưu trữ" },
      { text: `${summary.draftPosts + summary.archivedPosts} bài`, options: { bold: true } },
      { text: `${summary.draftPosts} bản nháp chưa công bố · ${summary.archivedPosts} bài lưu trữ` }
    ],
    [
      { text: "Bình luận đã phê duyệt" },
      { text: `${summary.approvedComments} bình luận`, options: { bold: true, color: BRAND_INDIGO } },
      { text: "Phản hồi tích cực từ cộng đồng độc giả" }
    ],
    [
      { text: "Bình luận chờ kiểm duyệt" },
      { text: `${summary.pendingComments} bình luận`, options: { bold: true, color: BRAND_AMBER } },
      { text: "Cần quản trị viên kiểm tra nội dung trước khi hiển thị" }
    ],
    [
      { text: "Danh mục & Thẻ gắn" },
      { text: `${summary.totalCategories} DM / ${summary.totalTags} Tags`, options: { bold: true } },
      { text: "Phân loại kiến trúc chủ đề và từ khóa bài viết" }
    ]
  ];

  slide2.addTable(tableRows, {
    x: 0.8,
    y: 1.3,
    w: 8.4,
    rowH: 0.42,
    fontSize: 11,
    fontFace: "Arial",
    color: BRAND_DARK,
    fill: { color: "FFFFFF" },
    border: { pt: 1, color: "E2E8F0" }
  });

  slide2.addText("PDQ Portfolio Blog System · Slide 2", {
    x: 0.8,
    y: 5.0,
    w: 8.4,
    h: 0.3,
    fontSize: 9,
    fontFace: "Arial",
    color: "94A3B8",
    align: "right"
  });

  // Write Real .pptx file
  const fileName = `Bao-cao-thong-ke-${new Date().toISOString().slice(0, 10)}.pptx`;
  await pptx.writeFile({ fileName });
}

/**
 * 2. Xuất biểu đồ Xu hướng Lượt xem (Views Trend PPTX)
 */
export async function exportViewsTrendPptx(
  dataPoints: ViewsTrendPoint[],
  periodLabel: string
) {
  const pptx = new pptxgen();
  pptx.layout = "LAYOUT_16x9";
  pptx.title = `Xu hướng Lượt xem (${periodLabel})`;

  const slide = pptx.addSlide();
  slide.background = { color: "F8FAFC" };

  slide.addText(`XU HƯỚNG LƯỢT XEM & ĐỘC GIẢ (${periodLabel.toUpperCase()})`, {
    x: 0.8,
    y: 0.5,
    w: 8.4,
    h: 0.5,
    fontSize: 18,
    fontFace: "Arial",
    bold: true,
    color: BRAND_ROSE
  });

  const categories = dataPoints.map((p) => p.dateLabel);
  const viewsData = dataPoints.map((p) => p.views);
  const readersData = dataPoints.map((p) => p.uniqueReaders);

  if (categories.length > 0) {
    const chartData = [
      {
        name: "Lượt xem (Views)",
        labels: categories,
        values: viewsData
      },
      {
        name: "Độc giả (Unique Readers)",
        labels: categories,
        values: readersData
      }
    ];

    slide.addChart(pptx.ChartType.line, chartData, {
      x: 0.8,
      y: 1.1,
      w: 8.4,
      h: 3.8,
      showTitle: false,
      showLegend: true,
      legendPos: "b",
      chartColors: [BRAND_ROSE, BRAND_INDIGO],
      lineSize: 3,
      lineSmooth: true
    });
  }

  slide.addText("PDQ Portfolio Blog System · Xuất slide định dạng PowerPoint (.pptx)", {
    x: 0.8,
    y: 5.0,
    w: 8.4,
    h: 0.3,
    fontSize: 9,
    fontFace: "Arial",
    color: "94A3B8",
    align: "right"
  });

  await pptx.writeFile({ fileName: `views-trend-${periodLabel}.pptx` });
}

/**
 * 3. Xuất biểu đồ Phân bổ Danh mục (Category Distribution PPTX)
 */
export async function exportCategoryDistributionPptx(
  categoriesData: CategoryAnalyticsItem[],
  metric: "views" | "posts"
) {
  const pptx = new pptxgen();
  pptx.layout = "LAYOUT_16x9";
  pptx.title = "Phân bổ Bài viết & Lượt xem theo Chủ đề";

  const slide = pptx.addSlide();
  slide.background = { color: "F8FAFC" };

  const metricTitle = metric === "views" ? "Tổng lượt xem" : "Số lượng bài viết";

  slide.addText(`PHÂN BỔ THEO CHỦ ĐỀ (${metricTitle.toUpperCase()})`, {
    x: 0.8,
    y: 0.5,
    w: 8.4,
    h: 0.5,
    fontSize: 18,
    fontFace: "Arial",
    bold: true,
    color: BRAND_ROSE
  });

  const categories = categoriesData.map((c) => c.categoryName);
  const dataVals = categoriesData.map((c) => (metric === "views" ? c.totalViews : c.postCount));

  if (categories.length > 0) {
    const chartData = [
      {
        name: metricTitle,
        labels: categories,
        values: dataVals
      }
    ];

    slide.addChart(pptx.ChartType.bar, chartData, {
      x: 0.8,
      y: 1.1,
      w: 8.4,
      h: 3.8,
      showTitle: false,
      showLegend: false,
      chartColors: [BRAND_AMBER, BRAND_ROSE, BRAND_INDIGO, BRAND_EMERALD, "06B6D4", "8B5CF6"]
    });
  }

  slide.addText("PDQ Portfolio Blog System · Xuất slide định dạng PowerPoint (.pptx)", {
    x: 0.8,
    y: 5.0,
    w: 8.4,
    h: 0.3,
    fontSize: 9,
    fontFace: "Arial",
    color: "94A3B8",
    align: "right"
  });

  await pptx.writeFile({ fileName: `category-analytics-${metric}.pptx` });
}

/**
 * 4. Xuất biểu đồ Tương tác Bình luận (Comments Engagement PPTX)
 */
export async function exportCommentsTrendPptx(
  commentsData: MonthlyCommentsItem[],
  year: string | number
) {
  const pptx = new pptxgen();
  pptx.layout = "LAYOUT_16x9";
  pptx.title = `Tương tác Bình luận Độc giả Năm ${year}`;

  const slide = pptx.addSlide();
  slide.background = { color: "F8FAFC" };

  slide.addText(`TƯƠNG TÁC BÌNH LUẬN ĐỘC GIẢ NĂM ${year}`, {
    x: 0.8,
    y: 0.5,
    w: 8.4,
    h: 0.5,
    fontSize: 18,
    fontFace: "Arial",
    bold: true,
    color: BRAND_ROSE
  });

  const months = commentsData.map((c) => c.monthLabel);
  const totalComments = commentsData.map((c) => c.totalComments);
  const approvedComments = commentsData.map((c) => c.approvedComments);

  if (months.length > 0) {
    const chartData = [
      {
        name: "Tổng bình luận gửi về",
        labels: months,
        values: totalComments
      },
      {
        name: "Bình luận đã phê duyệt",
        labels: months,
        values: approvedComments
      }
    ];

    slide.addChart(pptx.ChartType.line, chartData, {
      x: 0.8,
      y: 1.1,
      w: 8.4,
      h: 3.8,
      showTitle: false,
      showLegend: true,
      legendPos: "b",
      chartColors: [BRAND_EMERALD, BRAND_INDIGO],
      lineSize: 3,
      lineSmooth: true
    });
  }

  slide.addText("PDQ Portfolio Blog System · Xuất slide định dạng PowerPoint (.pptx)", {
    x: 0.8,
    y: 5.0,
    w: 8.4,
    h: 0.3,
    fontSize: 9,
    fontFace: "Arial",
    color: "94A3B8",
    align: "right"
  });

  await pptx.writeFile({ fileName: `monthly-comments-${year}.pptx` });
}

/**
 * 5. Xuất biểu đồ Cơ cấu Trạng thái Bài viết (Post Status Distribution PPTX)
 */
export async function exportPostStatusPptx(summary: AdminAnalyticsSummary) {
  const pptx = new pptxgen();
  pptx.layout = "LAYOUT_16x9";
  pptx.title = "Cơ cấu Trạng thái Bài viết";

  const slide = pptx.addSlide();
  slide.background = { color: "F8FAFC" };

  slide.addText("CƠ CẤU TRẠNG THÁI BÀI VIẾT", {
    x: 0.8,
    y: 0.5,
    w: 8.4,
    h: 0.5,
    fontSize: 18,
    fontFace: "Arial",
    bold: true,
    color: BRAND_ROSE
  });

  const chartData = [
    {
      name: "Trạng thái",
      labels: ["Đã xuất bản", "Bản nháp", "Lưu trữ"],
      values: [
        summary.publishedPosts || 0,
        summary.draftPosts || 0,
        summary.archivedPosts || 0
      ]
    }
  ];

  slide.addChart(pptx.ChartType.doughnut, chartData, {
    x: 1.2,
    y: 1.1,
    w: 7.6,
    h: 3.8,
    showTitle: false,
    showLegend: true,
    legendPos: "b",
    chartColors: [BRAND_EMERALD, BRAND_AMBER, "94A3B8"],
    showPercent: true
  });

  slide.addText("PDQ Portfolio Blog System · Xuất slide định dạng PowerPoint (.pptx)", {
    x: 0.8,
    y: 5.0,
    w: 8.4,
    h: 0.3,
    fontSize: 9,
    fontFace: "Arial",
    color: "94A3B8",
    align: "right"
  });

  await pptx.writeFile({ fileName: "post-status-distribution.pptx" });
}

/**
 * 6. Xuất biểu đồ Thống kê Email & Hộp thư Liên hệ (Contacts Trend PPTX)
 */
export async function exportContactsTrendPptx(
  contactsData: MonthlyContactsItem[],
  year: string | number
) {
  const pptx = new pptxgen();
  pptx.layout = "LAYOUT_16x9";
  pptx.title = `Thống kê Email & Hộp thư Liên hệ Năm ${year}`;

  const slide = pptx.addSlide();
  slide.background = { color: "F8FAFC" };

  slide.addText(`THỐNG KÊ EMAIL & HỘP THƯ LIÊN HỆ NĂM ${year}`, {
    x: 0.8,
    y: 0.5,
    w: 8.4,
    h: 0.5,
    fontSize: 18,
    fontFace: "Arial",
    bold: true,
    color: BRAND_ROSE
  });

  const months = contactsData.map((c) => c.monthLabel);
  const totalMessages = contactsData.map((c) => c.totalMessages);
  const repliedMessages = contactsData.map((c) => c.repliedMessages);
  const unreadMessages = contactsData.map((c) => c.unreadMessages);

  if (months.length > 0) {
    const chartData = [
      {
        name: "Tổng email / tin nhắn nhận",
        labels: months,
        values: totalMessages
      },
      {
        name: "Đã phản hồi",
        labels: months,
        values: repliedMessages
      },
      {
        name: "Chưa đọc",
        labels: months,
        values: unreadMessages
      }
    ];

    slide.addChart(pptx.ChartType.line, chartData, {
      x: 0.8,
      y: 1.1,
      w: 8.4,
      h: 3.8,
      showTitle: false,
      showLegend: true,
      legendPos: "b",
      chartColors: [BRAND_ROSE, BRAND_EMERALD, BRAND_AMBER],
      lineSize: 3,
      lineSmooth: true
    });
  }

  slide.addText("PDQ Portfolio Blog System · Xuất slide định dạng PowerPoint (.pptx)", {
    x: 0.8,
    y: 5.0,
    w: 8.4,
    h: 0.3,
    fontSize: 9,
    fontFace: "Arial",
    color: "94A3B8",
    align: "right"
  });

  await pptx.writeFile({ fileName: `monthly-contacts-${year}.pptx` });
}


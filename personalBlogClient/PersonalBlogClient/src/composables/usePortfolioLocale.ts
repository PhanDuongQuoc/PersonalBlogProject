import { computed, ref } from 'vue';

export type PortfolioLocale = 'vi' | 'en';

const storedLocale = typeof window === 'undefined' ? null : window.localStorage.getItem('portfolio-locale');
const locale = ref<PortfolioLocale>(storedLocale === 'en' ? 'en' : 'vi');

const messages = {
  en: {
    home: 'Home', about: 'About', topics: 'Topics', posts: 'Posts', views: 'Views', contact: 'Contact', resume: 'Resume', author: 'Author',
    heroIam: "I'm a",
    heroLead: 'Crafting web', heroMiddle: 'experiences that', heroEnd: 'perform.',
    heroDescription: "Welcome to {name}'s public space for published notes, practical ideas, and web development stories.",
    viewPosts: 'View posts', sayHello: 'Say hello', browseByCategory: 'Browse by category',
    topicsDescription: 'Topics calculated directly from published posts in the database.', profileDescription: 'This public profile is loaded from the author record in the database.',
    username: 'Username', email: 'Email', fromTheBlog: 'From the blog', featuredPosts: 'Featured Posts', publishedPosts: 'published posts',
    recent: 'Recent', readLatest: 'Read the latest thoughts and lessons from this project.', articlesPreparing: 'New articles are being prepared. Check back soon.',
    getInTouch: 'Get In Touch', collaborate: 'Have a question or want to collaborate?', loading: 'Loading...',
    loadFailed: 'Unable to load data right now. Please try again later.', lightMode: 'Use light mode', darkMode: 'Use dark mode', goTop: 'Go to the top of the page',

    // About page specific
    aboutPageEyebrow: 'About Me',
    aboutPageTitle: 'Passionate Web Developer & Software Craftsman',
    aboutPageBio: 'Building robust backend architectures and sleek modern frontend interfaces.',
    myStoryTitle: 'My Journey & Philosophy',
    myStorySubtitle: 'From initial lines of code to fullstack software architecture',
    techStackTitle: 'Technical Skills & Technologies',
    techStackSubtitle: 'Specialized toolkit and proficiency across multiple web development layers',
    allCategories: 'All Categories',
    workExperienceTitle: 'Work Experience & Key Projects',
    workExperienceSubtitle: 'Milestones, responsibilities, and delivered impact throughout my career',
    educationTitle: 'Education & Certifications',
    educationSubtitle: 'Academic background and professional development achievements',
    yearsExpLabel: 'Years of Experience',
    publishedPostsLabel: 'Articles Published',
    topicsExploredLabel: 'Topics Explored',
    totalViewsLabel: 'Cumulative Views',
    downloadCv: 'Download CV',
    locationLabel: 'Location',
    phoneLabel: 'Phone',
    present: 'Present',
    currentRole: 'Current Position',
    keyResponsibilities: 'Key responsibilities & accomplishments',
    technologiesUsed: 'Technologies & Tools',
    connectPrompt: 'Interested in working together or discussing opportunities? Feel free to reach out!',

    // Post Detail specific
    backToHome: 'Back to home',
    backToPosts: 'Back to articles',
    writtenBy: 'Written by',
    publishedOn: 'Published on',
    viewsCount: 'views',
    minRead: 'min read',
    tagsTitle: 'Tags',
    relatedArticles: 'Related Articles',
    commentsSection: 'Discussion & Comments',
    leaveAComment: 'Leave a Comment',
    nameField: 'Your Name *',
    emailField: 'Your Email (optional)',
    commentField: 'Write your thoughts... *',
    postCommentBtn: 'Post Comment',
    postingComment: 'Posting...',
    commentSuccessMsg: 'Thank you! Your comment has been posted.',
    noCommentsMessage: 'No comments yet. Be the first to share your thoughts!',
    tableOfContents: 'Contents',
    sections: 'sections',
    viewDossier: 'View Dossier & About',
    newsletterTitle: 'The Developer Dispatch',
    newsletterDesc: 'Get essays like this in your inbox every Sunday morning. Curated technical insights on software & web performance.',
    subscribeBtn: 'Subscribe',
    subscribeSuccessMsg: 'Thank you for subscribing!',
    copiedLink: 'Link copied to clipboard!',
    followBtn: 'Follow'
  },
  vi: {
    home: 'Trang chủ', about: 'Giới thiệu', topics: 'Chủ đề', posts: 'Bài viết', views: 'Lượt xem', contact: 'Liên hệ', resume: 'Hồ sơ', author: 'Tác giả',
    heroIam: 'Mình là một',
    heroLead: 'Xây dựng trải nghiệm', heroMiddle: 'web thật sự', heroEnd: 'hiệu quả.',
    heroDescription: 'Chào mừng bạn đến với không gian chia sẻ công khai của {name}, nơi lưu lại ghi chú, ý tưởng thực tế và câu chuyện phát triển web.',
    viewPosts: 'Xem bài viết', sayHello: 'Liên hệ', browseByCategory: 'Khám phá theo danh mục',
    topicsDescription: 'Các chủ đề được tổng hợp trực tiếp từ những bài viết đã xuất bản.', profileDescription: 'Thông tin hồ sơ công khai được tải từ dữ liệu tác giả.',
    username: 'Tên người dùng', email: 'Email', fromTheBlog: 'Từ blog', featuredPosts: 'Bài viết nổi bật', publishedPosts: 'bài viết đã xuất bản',
    readLatest: 'Đọc những chia sẻ và bài học mới nhất từ dự án này.', articlesPreparing: 'Các bài viết mới đang được chuẩn bị. Hãy quay lại sớm nhé.', recent: 'Gần đây',
    getInTouch: 'Liên hệ', collaborate: 'Bạn có câu hỏi hoặc muốn hợp tác cùng mình?', loading: 'Đang tải...',
    loadFailed: 'Không thể tải dữ liệu lúc này. Vui lòng thử lại sau.', lightMode: 'Chuyển sang giao diện sáng', darkMode: 'Chuyển sang giao diện tối', goTop: 'Về đầu trang',

    // About page specific
    aboutPageEyebrow: 'Giới thiệu bản thân',
    aboutPageTitle: 'Lập trình viên Web & Kỹ sư Phần mềm Đam mê Sáng tạo',
    aboutPageBio: 'Xây dựng kiến trúc backend vững chắc và giao diện frontend hiện đại, mượt mà.',
    myStoryTitle: 'Hành trình & Triết lý Lập trình',
    myStorySubtitle: 'Từ những dòng code đầu tiên đến kiến trúc phát triển phần mềm toàn diện',
    techStackTitle: 'Kỹ năng Chuyên môn & Công nghệ',
    techStackSubtitle: 'Bộ công cụ chuyên sâu và mức độ thành thạo qua các tầng phát triển web',
    allCategories: 'Tất cả danh mục',
    workExperienceTitle: 'Kinh nghiệm Làm việc & Dự án Tiêu biểu',
    workExperienceSubtitle: 'Các mốc thời gian, trách nhiệm và thành tựu đã đạt được',
    educationTitle: 'Trình độ Học vấn & Bằng cấp',
    educationSubtitle: 'Nền tảng đào tạo chính quy và các chứng chỉ chuyên ngành',
    yearsExpLabel: 'Năm kinh nghiệm',
    publishedPostsLabel: 'Bài viết đã đăng',
    topicsExploredLabel: 'Chủ đề chia sẻ',
    totalViewsLabel: 'Lượt xem tích lũy',
    downloadCv: 'Tải CV / Hồ sơ',
    locationLabel: 'Địa điểm',
    phoneLabel: 'Điện thoại',
    present: 'Hiện tại',
    currentRole: 'Vị trí hiện tại',
    keyResponsibilities: 'Trách nhiệm & Thành tựu nổi bật',
    technologiesUsed: 'Công nghệ sử dụng',
    connectPrompt: 'Bạn đang tìm kiếm cộng sự phát triển dự án hoặc muốn trao đổi công việc? Hãy liên hệ ngay!',

    // Post Detail specific
    backToHome: 'Quay lại trang chủ',
    backToPosts: 'Tất cả bài viết',
    writtenBy: 'Tác giả',
    publishedOn: 'Đăng ngày',
    viewsCount: 'lượt xem',
    minRead: 'phút đọc',
    tagsTitle: 'Thẻ bài viết',
    relatedArticles: 'Bài viết liên quan',
    commentsSection: 'Bình luận & Thảo luận',
    leaveAComment: 'Để lại bình luận của bạn',
    nameField: 'Họ và tên của bạn *',
    emailField: 'Email (không bắt buộc)',
    commentField: 'Nội dung bình luận... *',
    postCommentBtn: 'Gửi bình luận',
    postingComment: 'Đang gửi...',
    commentSuccessMsg: 'Cảm ơn bạn! Bình luận của bạn đã được đăng thành công.',
    noCommentsMessage: 'Chưa có bình luận nào. Hãy là người đầu tiên chia sẻ suy nghĩ của bạn!',
    tableOfContents: 'Mục lục nội dung',
    sections: 'phần',
    viewDossier: 'Xem hồ sơ tác giả',
    newsletterTitle: 'Bản tin Lập trình & Kiến trúc',
    newsletterDesc: 'Nhận các bài phân tích chuyên sâu về kiến trúc phần mềm và tối ưu web vào mỗi sáng Chủ Nhật.',
    subscribeBtn: 'Đăng ký nhận tin',
    subscribeSuccessMsg: 'Cảm ơn bạn đã đăng ký nhận bản tin!',
    copiedLink: 'Đã sao chép liên kết vào clipboard!',
    followBtn: 'Theo dõi'
  }
} as const;

export const usePortfolioLocale = () => {
  const text = computed(() => messages[locale.value]);

  const setLocale = (value: PortfolioLocale) => {
    locale.value = value;
    window.localStorage.setItem('portfolio-locale', value);
  };

  return { locale, setLocale, text };
};

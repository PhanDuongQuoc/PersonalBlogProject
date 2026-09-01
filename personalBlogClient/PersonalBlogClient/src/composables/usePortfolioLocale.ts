import { computed, ref } from 'vue';

export type PortfolioLocale = 'en' | 'vi';

const storedLocale = typeof window === 'undefined' ? null : window.localStorage.getItem('portfolio-locale');
const locale = ref<PortfolioLocale>(storedLocale === 'vi' ? 'vi' : 'en');

const messages = {
  en: {
    home: 'Home', about: 'About', topics: 'Topics', posts: 'Posts', views: 'Views', contact: 'Contact', resume: 'Resume', author: 'Author',
    heroLead: 'Crafting web', heroMiddle: 'experiences that', heroEnd: 'perform.',
    heroDescription: "Welcome to {name}'s public space for published notes, practical ideas, and web development stories.",
    viewPosts: 'View posts', sayHello: 'Say hello', browseByCategory: 'Browse by category',
    topicsDescription: 'Topics calculated directly from published posts in the database.', profileDescription: 'This public profile is loaded from the author record in the database.',
    username: 'Username', email: 'Email', fromTheBlog: 'From the blog', featuredPosts: 'Featured Posts', publishedPosts: 'published posts',
    readLatest: 'Read the latest thoughts and lessons from this project.', articlesPreparing: 'New articles are being prepared. Check back soon.', recent: 'Recent',
    getInTouch: 'Get In Touch', collaborate: 'Have a question or want to collaborate?', loading: 'Loading portfolio...',
    loadFailed: 'Unable to load the portfolio right now. Please try again later.', lightMode: 'Use light mode', darkMode: 'Use dark mode', goTop: 'Go to the top of the page'
  },
  vi: {
    home: 'Trang chủ', about: 'Giới thiệu', topics: 'Chủ đề', posts: 'Bài viết', views: 'Lượt xem', contact: 'Liên hệ', resume: 'Hồ sơ', author: 'Tác giả',
    heroLead: 'Xây dựng trải nghiệm', heroMiddle: 'web thật sự', heroEnd: 'hiệu quả.',
    heroDescription: 'Chào mừng bạn đến với không gian chia sẻ công khai của {name}, nơi lưu lại ghi chú, ý tưởng thực tế và câu chuyện phát triển web.',
    viewPosts: 'Xem bài viết', sayHello: 'Liên hệ', browseByCategory: 'Khám phá theo danh mục',
    topicsDescription: 'Các chủ đề được tổng hợp trực tiếp từ những bài viết đã xuất bản.', profileDescription: 'Thông tin hồ sơ công khai được tải từ dữ liệu tác giả.',
    username: 'Tên người dùng', email: 'Email', fromTheBlog: 'Từ blog', featuredPosts: 'Bài viết nổi bật', publishedPosts: 'bài viết đã xuất bản',
    readLatest: 'Đọc những chia sẻ và bài học mới nhất từ dự án này.', articlesPreparing: 'Các bài viết mới đang được chuẩn bị. Hãy quay lại sớm nhé.', recent: 'Gần đây',
    getInTouch: 'Liên hệ', collaborate: 'Bạn có câu hỏi hoặc muốn hợp tác cùng mình?', loading: 'Đang tải portfolio...',
    loadFailed: 'Không thể tải portfolio lúc này. Vui lòng thử lại sau.', lightMode: 'Chuyển sang giao diện sáng', darkMode: 'Chuyển sang giao diện tối', goTop: 'Về đầu trang'
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

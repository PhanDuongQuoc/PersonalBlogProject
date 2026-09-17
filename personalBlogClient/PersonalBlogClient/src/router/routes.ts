import type { RouteRecordRaw } from "vue-router";

const routes: RouteRecordRaw[] = [
  {
    path: "/login",
    component: () => import("@/layouts/AuthLayout.vue"),
    children: [
      {
        path: "",
        component: () => import("@/pages/LoginPage.vue"),
        meta: { guestOnly: true, title: "Đăng nhập Quản trị" }
      }
    ]
  },
  {
    path: "/forgot-password",
    component: () => import("@/layouts/AuthLayout.vue"),
    children: [
      {
        path: "",
        component: () => import("@/pages/ForgotPasswordPage.vue"),
        meta: { guestOnly: true, title: "Quên mật khẩu" }
      }
    ]
  },
  {
    path: "/admin",
    component: () => import("@/layouts/AdminLayout.vue"),
    meta: { requiresAuth: true },
    children: [
      {
        path: "",
        component: () => import("@/pages/admin/AdminDashboardPage.vue"),
        meta: { requiresAuth: true, title: "Bảng điều khiển Quản trị" }
      },
      {
        path: "analytics",
        component: () => import("@/pages/admin/AdminAnalyticsPage.vue"),
        meta: { requiresAuth: true, title: "Thống kê & Báo cáo" }
      },
      {
        path: "posts",
        component: () => import("@/pages/admin/AdminPostsPage.vue"),
        meta: { requiresAuth: true, title: "Quản lý Bài viết" }
      },
      {
        path: "categories",
        component: () => import("@/pages/admin/AdminCategoriesPage.vue"),
        meta: { requiresAuth: true, title: "Chủ đề & Danh mục" }
      },
      {
        path: "tags",
        component: () => import("@/pages/admin/AdminTagsPage.vue"),
        meta: { requiresAuth: true, title: "Quản lý Thẻ" }
      },
      {
        path: "comments",
        component: () => import("@/pages/admin/AdminCommentsPage.vue"),
        meta: { requiresAuth: true, title: "Quản lý Bình luận" }
      },
      {
        path: "profile",
        component: () => import("@/pages/admin/AdminProfilePage.vue"),
        meta: { requiresAuth: true, title: "Hồ sơ Tác giả" }
      },
      {
        path: "skills",
        component: () => import("@/pages/admin/AdminSkillsPage.vue"),
        meta: { requiresAuth: true, title: "Kỹ năng & Học vấn" }
      },
      {
        path: "security",
        component: () => import("@/pages/admin/AdminSecurityPage.vue"),
        meta: { requiresAuth: true, title: "Bảo mật & Tài khoản" }
      },
      {
        path: "contacts",
        component: () => import("@/pages/admin/AdminContactsPage.vue"),
        meta: { requiresAuth: true, title: "Hộp thư liên hệ" }
      },
      {
        path: "settings",
        component: () => import("@/pages/admin/AdminSettingsPage.vue"),
        meta: { requiresAuth: true, title: "Cài đặt Hệ thống" }
      }
    ]
  },
  {
    path: "/home",
    component: () => import("@/layouts/UserLayout.vue"),
    children: [
      { path: "", component: () => import("@/pages/user/UserHomePage.vue") }
    ]
  },
  {
    path: "/about",
    component: () => import("@/layouts/UserLayout.vue"),
    children: [
      { path: "", component: () => import("@/pages/user/UserAboutPage.vue") }
    ]
  },
  {
    path: "/topics",
    component: () => import("@/layouts/UserLayout.vue"),
    children: [
      { path: "", component: () => import("@/pages/user/UserTopicsPage.vue") }
    ]
  },
  {
    path: "/topics/:slug",
    component: () => import("@/layouts/UserLayout.vue"),
    children: [
      { path: "", component: () => import("@/pages/user/UserTopicDetailPage.vue") }
    ]
  },
  {
    path: "/posts/:slug",
    component: () => import("@/layouts/UserLayout.vue"),
    children: [
      { path: "", component: () => import("@/pages/user/UserPostDetailPage.vue") }
    ]
  },
  {
    path: "/contact",
    component: () => import("@/layouts/UserLayout.vue"),
    children: [
      { path: "", component: () => import("@/pages/user/UserContactPage.vue") }
    ]
  },
  {
    path: "/user",
    redirect: "/home"
  },
  {
    path: "/",
    redirect: "/home"
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: "/:catchAll(.*)*",
    component: () => import("@/pages/ErrorNotFound.vue")
  }
];

export default routes;

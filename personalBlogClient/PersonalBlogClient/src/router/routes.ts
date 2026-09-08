import type { RouteRecordRaw } from "vue-router";

const routes: RouteRecordRaw[] = [
  {
    path: "/login",
    component: () => import("@/layouts/AuthLayout.vue"),
    children: [
      { path: "", component: () => import("@/pages/LoginPage.vue") }
    ]
  },
  {
    path: "/admin",
    component: () => import("@/layouts/AdminLayout.vue"),
    children: [
      { path: "", component: () => import("@/pages/admin/AdminDashboardPage.vue") }
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
    path: "/posts/:slug",
    component: () => import("@/layouts/UserLayout.vue"),
    children: [
      { path: "", component: () => import("@/pages/user/UserPostDetailPage.vue") }
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

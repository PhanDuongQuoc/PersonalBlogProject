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
    path: "/user",
    component: () => import("@/layouts/UserLayout.vue"),
    children: [
      { path: "", component: () => import("@/pages/user/UserHomePage.vue") }
    ]
  },
  {
    path: "/",
    redirect: "/login"
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: "/:catchAll(.*)*",
    component: () => import("@/pages/ErrorNotFound.vue")
  }
];

export default routes;

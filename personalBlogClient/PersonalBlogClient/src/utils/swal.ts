import Swal, { type SweetAlertIcon } from "sweetalert2";

// Custom SweetAlert2 theme matching Modern Editorial Minimalist design tokens
const customSwal = Swal.mixin({
  customClass: {
    popup: "editorial-swal-popup",
    title: "editorial-swal-title",
    htmlContainer: "editorial-swal-html",
    confirmButton: "editorial-swal-btn-confirm",
    cancelButton: "editorial-swal-btn-cancel",
    denyButton: "editorial-swal-btn-deny",
    actions: "editorial-swal-actions"
  },
  buttonsStyling: false
});

/**
 * Hiển thị dialog xác nhận hành động (ví dụ: Xóa bài viết, đổi trạng thái)
 */
export async function swalConfirm(options: {
  title: string;
  text?: string;
  confirmButtonText?: string;
  cancelButtonText?: string;
  icon?: SweetAlertIcon;
  isDanger?: boolean;
}): Promise<boolean> {
  const result = await customSwal.fire({
    title: options.title,
    text: options.text,
    icon: options.icon ?? "warning",
    showCancelButton: true,
    confirmButtonText: options.confirmButtonText ?? "Xác nhận",
    cancelButtonText: options.cancelButtonText ?? "Hủy bỏ",
    reverseButtons: true,
    focusCancel: true,
    customClass: {
      popup: "editorial-swal-popup",
      title: "editorial-swal-title",
      htmlContainer: "editorial-swal-html",
      confirmButton: options.isDanger ? "editorial-swal-btn-danger" : "editorial-swal-btn-confirm",
      cancelButton: "editorial-swal-btn-cancel",
      actions: "editorial-swal-actions"
    }
  });

  return result.isConfirmed;
}

/**
 * Hiển thị thông báo thành công
 */
export function swalSuccess(title: string, text?: string) {
  return customSwal.fire({
    title,
    text,
    icon: "success",
    timer: 2500,
    showConfirmButton: true,
    confirmButtonText: "Đóng"
  });
}

/**
 * Hiển thị thông báo lỗi
 */
export function swalError(title: string, text?: string) {
  return customSwal.fire({
    title,
    text,
    icon: "error",
    confirmButtonText: "Đã hiểu"
  });
}

/**
 * Hiển thị thông báo cảnh báo
 */
export function swalWarning(title: string, text?: string) {
  return customSwal.fire({
    title,
    text,
    icon: "warning",
    confirmButtonText: "Đã hiểu"
  });
}

/**
 * Hiển thị Toast góc trên bên phải
 */
export function swalToast(title: string, icon: SweetAlertIcon = "success") {
  const toastMixin = Swal.mixin({
    toast: true,
    position: "top-end",
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true,
    customClass: {
      popup: "editorial-swal-toast"
    }
  });

  return toastMixin.fire({
    icon,
    title
  });
}

export default customSwal;

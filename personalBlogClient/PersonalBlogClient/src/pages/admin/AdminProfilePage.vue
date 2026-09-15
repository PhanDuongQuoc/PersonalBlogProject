<template>
  <q-page class="admin-profile-page">
    <q-inner-loading :showing="loading">
      <q-spinner-tail size="48px" color="rose-primary" />
    </q-inner-loading>

    <div v-if="!loading" class="profile-layout-container">
      <!-- 1. Hero Author Profile Card -->
      <section class="profile-hero-card">
        <div class="hero-left">
          <div class="hero-avatar-wrap">
            <img
              v-if="form.avatarUrl"
              :src="form.avatarUrl"
              :alt="form.displayName || 'Author Avatar'"
              class="hero-avatar-img"
              @error="onAvatarError"
            />
            <div v-else class="hero-avatar-fallback">
              {{ (form.displayName || 'A').charAt(0).toUpperCase() }}
            </div>
          </div>

          <div class="hero-info">
            <div class="hero-name-row">
              <h2 class="hero-display-name">{{ form.displayName || 'Chưa đặt tên tác giả' }}</h2>
              <span class="hero-role-badge">
                <q-icon name="fa-solid fa-user-pen" size="10px" class="q-mr-xs" />
                {{ profile?.role || 'Author' }}
              </span>
            </div>

            <div class="hero-meta-row">
              <span class="hero-job-title">
                <q-icon name="fa-solid fa-briefcase" size="11px" class="q-mr-xs text-rose" />
                {{ form.jobTitle || 'Fullstack Web Developer' }}
              </span>
              <span class="meta-dot">·</span>
              <span class="hero-location">
                <q-icon name="fa-solid fa-location-dot" size="11px" class="q-mr-xs text-rose" />
                {{ form.location || 'Việt Nam' }}
              </span>
              <span class="meta-dot">·</span>
              <span class="hero-exp">
                <q-icon name="fa-solid fa-clock-rotate-left" size="11px" class="q-mr-xs text-rose" />
                {{ form.yearsOfExperience || 0 }} năm kinh nghiệm
              </span>
            </div>
          </div>
        </div>

        <div class="hero-actions">
          <a
            href="/about"
            target="_blank"
            class="btn-preview-public"
            title="Xem trang giới thiệu tác giả công khai"
          >
            <span>Xem trang công khai</span>
            <q-icon name="fa-solid fa-arrow-up-right-from-square" size="11px" />
          </a>

          <button
            type="button"
            class="btn-save-primary"
            :disabled="saving"
            @click="handleSubmit"
          >
            <q-spinner v-if="saving" size="14px" color="white" />
            <q-icon v-else name="fa-solid fa-floppy-disk" size="13px" />
            <span>Lưu hồ sơ</span>
          </button>
        </div>
      </section>

      <!-- 2. Main Two-Column Editorial Grid -->
      <section class="profile-grid-section">
        <!-- Left Column: Personal Narrative & Bio -->
        <div class="profile-primary-col">
          <!-- Basic Info Card -->
          <div class="editorial-card">
            <div class="card-header">
              <q-icon name="fa-solid fa-id-card" size="14px" class="header-icon" />
              <span>THÔNG TIN CƠ BẢN</span>
            </div>
            <div class="card-body">
              <div class="form-row-2col">
                <div class="form-group-field">
                  <label class="field-label-mono">
                    HỌ VÀ TÊN HIỂN THỊ <span class="text-danger">*</span>
                  </label>
                  <input
                    v-model="form.displayName"
                    type="text"
                    placeholder="Nhập tên hiển thị của bạn..."
                    class="input-custom-field"
                  />
                </div>

                <div class="form-group-field">
                  <label class="field-label-mono">
                    CHỨC DANH CÔNG VIỆC <span class="text-danger">*</span>
                  </label>
                  <input
                    v-model="form.jobTitle"
                    type="text"
                    placeholder="Ví dụ: Senior Fullstack Developer & AI Engineer"
                    class="input-custom-field"
                  />
                </div>
              </div>

              <div class="form-row-2col">
                <div class="form-group-field">
                  <label class="field-label-mono">
                    SỐ NĂM KINH NGHIỆM
                  </label>
                  <input
                    v-model.number="form.yearsOfExperience"
                    type="number"
                    min="0"
                    max="50"
                    placeholder="3"
                    class="input-custom-field"
                  />
                </div>

                <div class="form-group-field">
                  <label class="field-label-mono">
                    ĐỊA ĐIỂM LÀM VIỆC
                  </label>
                  <input
                    v-model="form.location"
                    type="text"
                    placeholder="Ví dụ: Hồ Chí Minh, Việt Nam"
                    class="input-custom-field"
                  />
                </div>
              </div>

              <div class="form-row-2col">
                <div class="form-group-field">
                  <label class="field-label-mono">
                    EMAIL LIÊN HỆ
                  </label>
                  <input
                    v-model="form.email"
                    type="email"
                    placeholder="your-email@example.com"
                    class="input-custom-field"
                    disabled
                  />
                  <span class="field-hint">Email đăng nhập tài khoản hệ thống (không thể đổi)</span>
                </div>

                <div class="form-group-field">
                  <label class="field-label-mono">
                    SỐ ĐIỆN THOẠI
                  </label>
                  <input
                    v-model="form.phone"
                    type="text"
                    placeholder="0987 654 321"
                    class="input-custom-field"
                  />
                </div>
              </div>
            </div>
          </div>

          <!-- Short Bio Card -->
          <div class="editorial-card">
            <div class="card-header">
              <q-icon name="fa-solid fa-quote-left" size="14px" class="header-icon" />
              <span>TIỂU SỬ TÓM TẮT (SHORT BIO)</span>
            </div>
            <div class="card-body">
              <div class="form-group-field">
                <textarea
                  v-model="form.bio"
                  rows="3"
                  placeholder="1 - 2 câu ngắn gọn giới thiệu bản thân hiển thị trên thẻ tác giả ở chân bài viết và trang chủ..."
                  class="textarea-custom-field"
                ></textarea>
              </div>
            </div>
          </div>

          <!-- Full About Story Card -->
          <div class="editorial-card">
            <div class="card-header">
              <q-icon name="fa-solid fa-book-open" size="14px" class="header-icon" />
              <span>CÂU CHUYỆN & GIỚI THIỆU CHI TIẾT (ABOUT STORY)</span>
            </div>
            <div class="card-body">
              <div class="form-group-field">
                <textarea
                  v-model="form.aboutStory"
                  rows="8"
                  placeholder="Chia sẻ hành trình học tập, đam mê lập trình, định hướng công nghệ và sứ mệnh của bạn. Nội dung này sẽ hiển thị trang trọng trên trang /about..."
                  class="textarea-custom-field font-story"
                ></textarea>
              </div>
            </div>
          </div>
        </div>

        <!-- Right Column: Avatar, Resume & Social Links -->
        <div class="profile-secondary-col">
          <!-- Avatar Card with Live Preview -->
          <div class="editorial-card">
            <div class="card-header">
              <q-icon name="fa-solid fa-image" size="14px" class="header-icon" />
              <span>ẢNH ĐẠI DIỆN (AVATAR)</span>
            </div>
            <div class="card-body">
              <div class="avatar-preview-container">
                <div class="avatar-preview-circle">
                  <img
                    v-if="form.avatarUrl"
                    :src="form.avatarUrl"
                    :alt="form.displayName || 'Avatar Preview'"
                    class="preview-circle-img"
                    @error="onAvatarError"
                  />
                  <div v-else class="preview-circle-placeholder">
                    <q-icon name="fa-solid fa-user" size="32px" />
                  </div>
                </div>

                <div class="avatar-url-input-wrap">
                  <label class="field-label-mono">ĐƯỜNG DẪN ẢNH (AVATAR URL)</label>
                  <input
                    v-model="form.avatarUrl"
                    type="text"
                    placeholder="https://images.unsplash.com/..."
                    class="input-custom-field input-sm"
                  />
                  <span class="field-hint">Hỗ trợ link ảnh Unsplash, Cloudinary hoặc link ảnh trực tuyến</span>
                </div>
              </div>
            </div>
          </div>

          <!-- CV & Personal Website Card -->
          <div class="editorial-card">
            <div class="card-header">
              <q-icon name="fa-solid fa-file-pdf" size="14px" class="header-icon" />
              <span>HỒ SƠ NĂNG LỰC & WEBSITE</span>
            </div>
            <div class="card-body">
              <div class="form-group-field">
                <label class="field-label-mono">LINK TẢI CV / RESUME (PDF)</label>
                <input
                  v-model="form.cvUrl"
                  type="text"
                  placeholder="https://example.com/cv-phanduongquoc.pdf"
                  class="input-custom-field"
                />
              </div>

              <div class="form-group-field q-mt-md">
                <label class="field-label-mono">WEBSITE CÁ NHÂN (PORTFOLIO)</label>
                <input
                  v-model="form.websiteUrl"
                  type="text"
                  placeholder="https://pdq-personal-blog.vercel.app"
                  class="input-custom-field"
                />
              </div>
            </div>
          </div>

          <!-- Social Media Profiles Card -->
          <div class="editorial-card">
            <div class="card-header">
              <q-icon name="fa-solid fa-share-nodes" size="14px" class="header-icon" />
              <span>LIÊN KẾT MẠNG XÃ HỘI</span>
            </div>
            <div class="card-body">
              <div class="form-group-field">
                <label class="field-label-mono">
                  <q-icon name="fa-brands fa-github" size="12px" class="q-mr-xs text-dark" />
                  GITHUB PROFILE
                </label>
                <input
                  v-model="form.githubUrl"
                  type="text"
                  placeholder="https://github.com/phanduongquoc"
                  class="input-custom-field"
                />
              </div>

              <div class="form-group-field q-mt-md">
                <label class="field-label-mono">
                  <q-icon name="fa-brands fa-linkedin" size="12px" class="q-mr-xs text-indigo" />
                  LINKEDIN PROFILE
                </label>
                <input
                  v-model="form.linkedinUrl"
                  type="text"
                  placeholder="https://linkedin.com/in/phanduongquoc"
                  class="input-custom-field"
                />
              </div>

              <div class="form-group-field q-mt-md">
                <label class="field-label-mono">
                  <q-icon name="fa-brands fa-facebook" size="12px" class="q-mr-xs text-blue-8" />
                  FACEBOOK PROFILE
                </label>
                <input
                  v-model="form.facebookUrl"
                  type="text"
                  placeholder="https://facebook.com/..."
                  class="input-custom-field"
                />
              </div>

              <div class="form-group-field q-mt-md">
                <label class="field-label-mono">
                  <q-icon name="fa-brands fa-x-twitter" size="12px" class="q-mr-xs text-dark" />
                  TWITTER / X PROFILE
                </label>
                <input
                  v-model="form.twitterUrl"
                  type="text"
                  placeholder="https://x.com/..."
                  class="input-custom-field"
                />
              </div>
            </div>
          </div>
        </div>
      </section>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import type { AdminProfile, UpdateAdminProfilePayload } from "@/types/admin-profile";
import { adminProfileService } from "@/services/admin-profile.service";
import { swalSuccess, swalError } from "@/utils/swal";

const loading = ref(true);
const saving = ref(false);
const profile = ref<AdminProfile | null>(null);

const form = reactive<{
  displayName: string;
  avatarUrl: string;
  jobTitle: string;
  bio: string;
  aboutStory: string;
  location: string;
  phone: string;
  email: string;
  cvUrl: string;
  githubUrl: string;
  linkedinUrl: string;
  facebookUrl: string;
  twitterUrl: string;
  websiteUrl: string;
  yearsOfExperience: number | null;
}>({
  displayName: "",
  avatarUrl: "",
  jobTitle: "",
  bio: "",
  aboutStory: "",
  location: "",
  phone: "",
  email: "",
  cvUrl: "",
  githubUrl: "",
  linkedinUrl: "",
  facebookUrl: "",
  twitterUrl: "",
  websiteUrl: "",
  yearsOfExperience: 3
});

async function fetchProfile() {
  try {
    loading.value = true;
    const res = await adminProfileService.getProfile();
    profile.value = res;

    form.displayName = res.displayName || "";
    form.avatarUrl = res.avatarUrl || "";
    form.jobTitle = res.jobTitle || "";
    form.bio = res.bio || "";
    form.aboutStory = res.aboutStory || "";
    form.location = res.location || "";
    form.phone = res.phone || "";
    form.email = res.email || "";
    form.cvUrl = res.cvUrl || "";
    form.githubUrl = res.githubUrl || "";
    form.linkedinUrl = res.linkedinUrl || "";
    form.facebookUrl = res.facebookUrl || "";
    form.twitterUrl = res.twitterUrl || "";
    form.websiteUrl = res.websiteUrl || "";
    form.yearsOfExperience = res.yearsOfExperience;
  } catch (err: any) {
    const msg = err.response?.data?.message || "Không thể tải hồ sơ tác giả.";
    swalError("Lỗi tải hồ sơ", msg);
  } finally {
    loading.value = false;
  }
}

async function handleSubmit() {
  if (!form.displayName.trim()) {
    swalError("Thiếu họ tên", "Vui lòng nhập họ và tên hiển thị của tác giả.");
    return;
  }

  try {
    saving.value = true;
    const payload: UpdateAdminProfilePayload = {
      displayName: form.displayName.trim(),
      avatarUrl: form.avatarUrl.trim() || undefined,
      jobTitle: form.jobTitle.trim() || undefined,
      bio: form.bio.trim() || undefined,
      aboutStory: form.aboutStory.trim() || undefined,
      location: form.location.trim() || undefined,
      phone: form.phone.trim() || undefined,
      cvUrl: form.cvUrl.trim() || undefined,
      githubUrl: form.githubUrl.trim() || undefined,
      linkedinUrl: form.linkedinUrl.trim() || undefined,
      facebookUrl: form.facebookUrl.trim() || undefined,
      twitterUrl: form.twitterUrl.trim() || undefined,
      websiteUrl: form.websiteUrl.trim() || undefined,
      yearsOfExperience: form.yearsOfExperience ?? undefined
    };

    const updated = await adminProfileService.updateProfile(payload);
    profile.value = updated;
    swalSuccess("Cập nhật thành công!", "Hồ sơ tác giả đã được cập nhật thành công.");
  } catch (err: any) {
    const msg = err.response?.data?.message || "Đã xảy ra lỗi khi lưu hồ sơ.";
    swalError("Lỗi lưu hồ sơ", msg);
  } finally {
    saving.value = false;
  }
}

function onAvatarError(e: Event) {
  (e.target as HTMLImageElement).src =
    "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=400&auto=format&fit=crop&q=80";
}

onMounted(() => {
  fetchProfile();
});
</script>

<style scoped lang="scss">
.admin-profile-page {
  max-width: 100%;
  margin: 0 auto;
  padding: 8px 10px 30px;
  min-height: calc(100vh - 84px);
}

.profile-layout-container {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

/* 1. Hero Profile Card */
.profile-hero-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 16px;
  padding: 24px 28px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.02);
  flex-wrap: wrap;

  .hero-left {
    display: flex;
    align-items: center;
    gap: 20px;
    min-width: 0;

    .hero-avatar-wrap {
      width: 72px;
      height: 72px;
      border-radius: 50%;
      overflow: hidden;
      background: #fdf2f6;
      border: 2px solid #fce7f3;
      flex-shrink: 0;

      .hero-avatar-img {
        width: 100%;
        height: 100%;
        object-fit: cover;
      }

      .hero-avatar-fallback {
        width: 100%;
        height: 100%;
        display: flex;
        align-items: center;
        justify-content: center;
        font-family: var(--font-headline, sans-serif);
        font-size: 26px;
        font-weight: 800;
        color: #df266a;
      }
    }

    .hero-info {
      display: flex;
      flex-direction: column;
      gap: 4px;
      min-width: 0;

      .hero-name-row {
        display: flex;
        align-items: center;
        gap: 10px;

        .hero-display-name {
          font-family: var(--font-headline, sans-serif);
          font-size: 22px;
          font-weight: 800;
          color: #0b1326;
          margin: 0;
          line-height: 1.2;
        }

        .hero-role-badge {
          font-family: var(--font-mono, monospace);
          font-size: 10.5px;
          font-weight: 700;
          color: #df266a;
          background: #fdf2f6;
          border: 1px solid #fce7f3;
          padding: 3px 8px;
          border-radius: 6px;
          display: inline-flex;
          align-items: center;
        }
      }

      .hero-meta-row {
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 13px;
        color: #64748b;
        flex-wrap: wrap;

        .meta-dot {
          color: #cbd5e1;
        }
      }
    }
  }

  .hero-actions {
    display: flex;
    align-items: center;
    gap: 12px;

    .btn-preview-public {
      background: #ffffff;
      border: 1px solid #e2e8f0;
      color: #475569;
      font-family: var(--font-headline, sans-serif);
      font-size: 13px;
      font-weight: 600;
      padding: 9px 18px;
      border-radius: 9999px;
      text-decoration: none;
      display: inline-flex;
      align-items: center;
      gap: 8px;
      transition: all 0.2s ease;

      &:hover {
        background: #f1f5f9;
        color: #0b1326;
        border-color: #cbd5e1;
      }
    }

    .btn-save-primary {
      background: #df266a;
      color: #ffffff;
      font-family: var(--font-headline, sans-serif);
      font-size: 13.5px;
      font-weight: 700;
      padding: 9px 22px;
      border-radius: 9999px;
      border: none;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 8px;
      box-shadow: 0 4px 14px rgba(223, 38, 106, 0.28);
      transition: all 0.2s ease;

      &:hover:not(:disabled) {
        background: #be185d;
        transform: translateY(-1px);
        box-shadow: 0 6px 18px rgba(223, 38, 106, 0.35);
      }

      &:disabled {
        opacity: 0.6;
        cursor: not-allowed;
      }
    }
  }
}

/* 2. Grid Section */
.profile-grid-section {
  display: grid;
  grid-template-columns: 1fr 380px;
  gap: 20px;

  @media (max-width: 1024px) {
    grid-template-columns: 1fr;
  }
}

.profile-primary-col,
.profile-secondary-col {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

/* 3. Editorial Cards */
.editorial-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.02);

  .card-header {
    background: #f8fafc;
    border-bottom: 1px solid #e2e8f0;
    padding: 12px 20px;
    font-family: var(--font-mono, monospace);
    font-size: 11px;
    font-weight: 700;
    color: #475569;
    letter-spacing: 0.06em;
    display: flex;
    align-items: center;
    gap: 8px;

    .header-icon {
      color: #df266a;
    }
  }

  .card-body {
    padding: 20px;
  }
}

/* Form Styles */
.form-row-2col {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  margin-bottom: 14px;

  &:last-child {
    margin-bottom: 0;
  }

  @media (max-width: 640px) {
    grid-template-columns: 1fr;
  }
}

.form-group-field {
  display: flex;
  flex-direction: column;
  gap: 6px;

  .field-label-mono {
    font-family: var(--font-mono, monospace);
    font-size: 10.5px;
    font-weight: 700;
    color: #475569;
    letter-spacing: 0.05em;

    .text-danger {
      color: #e11d48;
    }
  }

  .field-hint {
    font-size: 11px;
    color: #94a3b8;
    margin-top: 2px;
  }
}

.input-custom-field {
  width: 100%;
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 10px 14px;
  font-family: var(--font-headline, sans-serif);
  font-size: 13.5px;
  color: #0b1326;
  outline: none;
  transition: all 0.2s ease;

  &:focus:not(:disabled) {
    border-color: #df266a;
    box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
  }

  &:disabled {
    background: #f8fafc;
    color: #94a3b8;
    cursor: not-allowed;
  }

  &.input-sm {
    font-size: 12.5px;
    padding: 8px 12px;
  }
}

.textarea-custom-field {
  width: 100%;
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 12px 14px;
  font-family: var(--font-body, sans-serif);
  font-size: 13.5px;
  line-height: 1.55;
  color: #0b1326;
  outline: none;
  resize: vertical;
  transition: all 0.2s ease;

  &:focus {
    border-color: #df266a;
    box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
  }

  &.font-story {
    font-family: var(--font-body, sans-serif);
    font-size: 14px;
    line-height: 1.7;
  }
}

/* Avatar Preview Container */
.avatar-preview-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;

  .avatar-preview-circle {
    width: 100px;
    height: 100px;
    border-radius: 50%;
    overflow: hidden;
    background: #f1f5f9;
    border: 3px solid #fce7f3;
    box-shadow: 0 4px 12px rgba(223, 38, 106, 0.15);

    .preview-circle-img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .preview-circle-placeholder {
      width: 100%;
      height: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
      color: #cbd5e1;
    }
  }

  .avatar-url-input-wrap {
    width: 100%;
    display: flex;
    flex-direction: column;
    gap: 4px;
  }
}
</style>

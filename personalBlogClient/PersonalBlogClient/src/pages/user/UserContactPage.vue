<template>
  <q-page class="portfolio-page">
    <main class="portfolio-content">
      <!-- 1. Editorial Breadcrumb -->
      <nav class="contact-breadcrumb">
        <router-link to="/home" class="bc-link">
          <q-icon name="arrow_back" size="16px" />
          <span>{{ text.home }}</span>
        </router-link>
        <span class="bc-separator">/</span>
        <span class="bc-current">{{ text.contact }}</span>
      </nav>

      <!-- 2. Main 2-Column Architectural Layout -->
      <div class="contact-grid-layout">
        <!-- Left Column: Availability, Dossier & Direct Coordinates -->
        <aside class="contact-context-column">
          <!-- Availability Micro-Badge Component (Skill-AI Mandate) -->
          <div class="availability-indicator-box">
            <span class="pulse-emerald-dot"></span>
            <span class="availability-text">{{ text.contactAvailableBadge }}</span>
          </div>

          <h1 class="contact-hero-title">
            {{ text.contactPageTitle }}
          </h1>

          <p class="contact-hero-desc">
            {{ text.contactPageDesc }}
          </p>

          <!-- Direct Channels Cards List -->
          <div class="direct-channels-stack">
            <!-- Channel 1: Primary Email with Copy -->
            <div class="channel-card">
              <div class="channel-icon-box">
                <q-icon name="mail" size="18px" />
              </div>
              <div class="channel-info">
                <span class="channel-label">{{ text.email.toUpperCase() }}</span>
                <a :href="`mailto:${contactProfile.email}`" class="channel-value email-link">
                  {{ contactProfile.email }}
                </a>
              </div>
              <button
                type="button"
                class="channel-action-btn"
                title="Copy email to clipboard"
                @click="copyToClipboard(contactProfile.email, text.copiedLink)"
              >
                <q-icon name="content_copy" size="15px" />
              </button>
            </div>

            <!-- Channel 2: Response Window SLA -->
            <div class="channel-card">
              <div class="channel-icon-box">
                <q-icon name="schedule" size="18px" />
              </div>
              <div class="channel-info">
                <span class="channel-label">{{ text.responseSla.toUpperCase() }}</span>
                <strong class="channel-value">{{ text.responseSlaDesc }}</strong>
              </div>
            </div>

            <!-- Channel 3: Location & Timezone -->
            <div class="channel-card">
              <div class="channel-icon-box">
                <q-icon name="public" size="18px" />
              </div>
              <div class="channel-info">
                <span class="channel-label">{{ text.locationTimezone.toUpperCase() }}</span>
                <strong class="channel-value">{{ contactProfile.location || text.locationTimezoneDesc }}</strong>
              </div>
            </div>
          </div>

          <!-- Social & Network Profiles Row -->
          <div class="social-channels-group">
            <a
              v-if="contactProfile.githubUrl"
              :href="contactProfile.githubUrl"
              target="_blank"
              rel="noopener noreferrer"
              class="social-pill-btn"
            >
              <q-icon name="code" size="15px" />
              <span>GitHub</span>
              <q-icon name="north_east" size="12px" class="arrow-up" />
            </a>

            <a
              v-if="contactProfile.linkedinUrl"
              :href="contactProfile.linkedinUrl"
              target="_blank"
              rel="noopener noreferrer"
              class="social-pill-btn"
            >
              <q-icon name="work_outline" size="15px" />
              <span>LinkedIn</span>
              <q-icon name="north_east" size="12px" class="arrow-up" />
            </a>

            <a
              v-if="contactProfile.cvUrl"
              :href="contactProfile.cvUrl"
              target="_blank"
              rel="noopener noreferrer"
              class="social-pill-btn cv-accent"
            >
              <q-icon name="description" size="15px" />
              <span>{{ text.downloadCv }}</span>
              <q-icon name="north_east" size="12px" class="arrow-up" />
            </a>
          </div>
        </aside>

        <!-- Right Column: Editorial Contact Form Card -->
        <section class="contact-form-column">
          <div class="contact-form-card">
            <div class="form-card-header">
              <div class="form-eyebrow">
                <q-icon name="send" size="14px" />
                <span>{{ text.contactPageEyebrow }}</span>
              </div>
              <h2 class="form-card-title">{{ text.sayHello }}</h2>
            </div>

            <!-- Success State Banner -->
            <div v-if="submissionStatus === 'success'" class="submission-alert-banner success-banner">
              <div class="alert-icon-circle">
                <q-icon name="check" size="20px" />
              </div>
              <div class="alert-text-block">
                <strong>{{ text.contactSuccess }}</strong>
                <p>{{ text.responseSlaDesc }}</p>
              </div>
              <button type="button" class="alert-reset-btn" @click="resetForm">
                <q-icon name="refresh" size="14px" />
                <span>Gửi tin nhắn khác</span>
              </button>
            </div>

            <!-- Error State Banner -->
            <div v-else-if="submissionStatus === 'error'" class="submission-alert-banner error-banner">
              <q-icon name="error_outline" size="20px" class="error-ico" />
              <div class="alert-text-block">
                <strong>{{ errorMessage || text.contactError }}</strong>
              </div>
              <button type="button" class="alert-close-btn" @click="submissionStatus = 'idle'">
                <q-icon name="close" size="14px" />
              </button>
            </div>

            <!-- The Form -->
            <form v-if="submissionStatus !== 'success'" @submit.prevent="handleSubmit" class="editorial-form">
              <!-- Web3Forms Honeypot Anti-Spam (Hidden) -->
              <input type="checkbox" name="botcheck" class="hidden-honeypot" v-model="form.botcheck" />

              <!-- Row 1: Name & Email -->
              <div class="form-dual-row">
                <!-- Name Field -->
                <div class="field-container">
                  <label class="field-label" for="contact-name">
                    {{ text.contactName }}
                  </label>
                  <input
                    id="contact-name"
                    v-model="form.name"
                    type="text"
                    required
                    class="editorial-input"
                    :placeholder="text.contactNamePlaceholder"
                    :disabled="isSubmitting"
                  />
                </div>

                <!-- Email Field -->
                <div class="field-container">
                  <label class="field-label" for="contact-email">
                    {{ text.contactEmail }}
                  </label>
                  <input
                    id="contact-email"
                    v-model="form.email"
                    type="email"
                    required
                    class="editorial-input"
                    :placeholder="text.contactEmailPlaceholder"
                    :disabled="isSubmitting"
                  />
                </div>
              </div>

              <!-- Row 2: Subject / Topic Category -->
              <div class="field-container">
                <label class="field-label" for="contact-subject">
                  {{ text.contactSubject }}
                </label>
                <input
                  id="contact-subject"
                  v-model="form.subject"
                  type="text"
                  class="editorial-input"
                  :placeholder="text.contactSubjectPlaceholder"
                  :disabled="isSubmitting"
                />
              </div>

              <!-- Row 3: Message Content -->
              <div class="field-container">
                <div class="label-with-meta">
                  <label class="field-label" for="contact-message">
                    {{ text.contactMessage }}
                  </label>
                  <span class="char-count">{{ form.message.length }} / 2000</span>
                </div>
                <textarea
                  id="contact-message"
                  v-model="form.message"
                  required
                  rows="6"
                  maxlength="2000"
                  class="editorial-textarea"
                  :placeholder="text.contactMessagePlaceholder"
                  :disabled="isSubmitting"
                ></textarea>
              </div>

              <!-- Form Footer: Submit Button & Security Note -->
              <div class="form-action-row">
                <button
                  type="submit"
                  :disabled="isSubmitting || !form.name.trim() || !form.email.trim() || !form.message.trim()"
                  class="editorial-submit-btn"
                >
                  <q-spinner v-if="isSubmitting" size="18px" color="dark" />
                  <template v-else>
                    <span>{{ text.contactSubmit }}</span>
                    <q-icon name="arrow_forward" size="16px" class="btn-arrow" />
                  </template>
                </button>

                <div class="form-trust-indicator">
                  <q-icon name="lock_outline" size="13px" />
                  <span>Chuyển tiếp tức thì qua Web3Forms</span>
                </div>
              </div>
            </form>
          </div>
        </section>
      </div>

      <!-- 3. Stylized Google Map Frame Section (Below the Contact Form) -->
      <section class="contact-map-section">
        <portfolio-section-title
          :eyebrow="text.mapSectionEyebrow"
          :title="text.mapSectionTitle"
          :description="text.mapSectionDesc"
        />
        <portfolio-location-map :location-name="contactProfile.location" />
      </section>
    </main>

    <portfolio-footer :email="contactProfile.email" />
  </q-page>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useQuasar } from 'quasar';
import api from '@/boot/ApiGateway/axios';
import PortfolioFooter from '@/components/portfolio/PortfolioFooter.vue';
import PortfolioLocationMap from '@/components/portfolio/PortfolioLocationMap.vue';
import PortfolioSectionTitle from '@/components/portfolio/PortfolioSectionTitle.vue';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
import type { PublicLandingResponse } from '@/types/public-landing';

const $q = useQuasar();
const route = useRoute();
const { text } = usePortfolioLocale();

// 1. Cấu hình Web3Forms Access Key (Bạn có thể thay bằng key cá nhân của bạn tại https://web3forms.com)
// Mặc định sử dụng Access Key công khai hoặc demo
const WEB3FORMS_ACCESS_KEY = '5f492b4c-9fcf-49b8-8097-f50f28e21ec3';

// 2. Profile tác giả (Đồng bộ trực tiếp từ Database PostgreSQL qua API)
const contactProfile = reactive({
  name: 'Phan Duong Quoc',
  email: 'phanduongquoc@example.com',
  location: 'TP. Hồ Chí Minh, Việt Nam',
  phone: '0987 654 321',
  githubUrl: 'https://github.com/phanduongquoc',
  linkedinUrl: 'https://linkedin.com/in/phanduongquoc',
  cvUrl: 'https://example.com/cv-phanduongquoc.pdf'
});

// 3. Form Reactive State
const form = reactive({
  name: '',
  email: '',
  subject: '',
  message: '',
  botcheck: false
});

const isSubmitting = ref(false);
const submissionStatus = ref<'idle' | 'success' | 'error'>('idle');
const errorMessage = ref('');

// 4. Fetch Profile từ API Backend để hiển thị chính xác email, location, social links
const loadAuthorData = async () => {
  try {
    const username = typeof route.query.username === 'string' ? route.query.username : undefined;
    const res = await api.get<PublicLandingResponse>('/public/landing', {
      params: { username }
    });
    if (res.data?.profile) {
      contactProfile.name = res.data.profile.name;
      contactProfile.email = res.data.profile.email;
      contactProfile.location = res.data.profile.location || contactProfile.location;
      contactProfile.phone = res.data.profile.phone || contactProfile.phone;
      contactProfile.githubUrl = res.data.profile.githubUrl || contactProfile.githubUrl;
      contactProfile.linkedinUrl = res.data.profile.linkedinUrl || contactProfile.linkedinUrl;
      contactProfile.cvUrl = res.data.profile.cvUrl || contactProfile.cvUrl;
    }
  } catch (err) {
    console.warn('Using default contact info fallback:', err);
  }
};

// 5. Xử lý Submit Form qua Web3Forms API
const handleSubmit = async () => {
  if (form.botcheck) return; // Anti-bot honeypot check
  if (!form.name.trim() || !form.email.trim() || !form.message.trim()) return;

  try {
    isSubmitting.value = true;
    errorMessage.value = '';

    const payload = {
      access_key: WEB3FORMS_ACCESS_KEY,
      name: form.name.trim(),
      email: form.email.trim(),
      subject: form.subject.trim() || `Tin nhắn liên hệ mới từ ${form.name.trim()} - Personal Blog`,
      message: form.message.trim(),
      from_name: 'Personal Portfolio Contact Page'
    };

    const response = await fetch('https://api.web3forms.com/submit', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json'
      },
      body: JSON.stringify(payload)
    });

    const result = await response.json();

    if (response.ok && result.success) {
      submissionStatus.value = 'success';
      $q.notify({
        type: 'positive',
        message: text.value.contactSuccess,
        position: 'top',
        timeout: 4000
      });
    } else {
      submissionStatus.value = 'error';
      errorMessage.value = result.message || text.value.contactError;
      $q.notify({
        type: 'negative',
        message: errorMessage.value,
        position: 'top',
        timeout: 4000
      });
    }
  } catch (err) {
    console.error('Web3Forms submit error:', err);
    submissionStatus.value = 'error';
    errorMessage.value = text.value.contactError;
  } finally {
    isSubmitting.value = false;
  }
};

const resetForm = () => {
  form.name = '';
  form.email = '';
  form.subject = '';
  form.message = '';
  form.botcheck = false;
  submissionStatus.value = 'idle';
};

const copyToClipboard = async (textToCopy: string, successMsg: string) => {
  try {
    await navigator.clipboard.writeText(textToCopy);
    $q.notify({
      type: 'positive',
      message: successMsg,
      position: 'top',
      timeout: 2500
    });
  } catch {
    console.error('Clipboard copy failed');
  }
};

onMounted(() => {
  window.scrollTo({ top: 0, behavior: 'instant' });
  loadAuthorData();
});
</script>

<style scoped lang="scss">
.portfolio-page {
  background: transparent;
  min-height: 100vh;
}

.portfolio-content {
  margin: 0 auto;
  max-width: 1140px;
  padding: 40px 32px 80px;
  box-sizing: border-box;
}

/* ==========================================================================
   1. BREADCRUMB
   ========================================================================== */
.contact-breadcrumb {
  display: flex;
  align-items: center;
  gap: 8px;
  font-family: var(--font-mono);
  font-size: 0.82rem;
  color: var(--text-muted);
  margin-bottom: 36px;

  .bc-link {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    color: var(--text-muted);
    text-decoration: none;
    transition: color 0.2s ease;

    &:hover {
      color: var(--accent-primary);
    }
  }

  .bc-separator {
    color: var(--border-subtle);
  }

  .bc-current {
    color: var(--text-primary);
    font-weight: 600;
  }
}

/* ==========================================================================
   2. TWO-COLUMN ARCHITECTURAL GRID
   ========================================================================== */
.contact-grid-layout {
  display: grid;
  grid-template-columns: 4.5fr 7.5fr;
  gap: 48px;
  align-items: start;
}

/* --------------------------------------------------------------------------
   LEFT COLUMN: CONTEXT & DIRECT COORDINATES
   -------------------------------------------------------------------------- */
.contact-context-column {
  display: flex;
  flex-direction: column;
}

/* Availability Micro-Badge Component (Skill_AIGEN Mandate) */
.availability-indicator-box {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  background: rgba(223, 38, 106, 0.08);
  border: 1px solid rgba(223, 38, 106, 0.25);
  border-radius: var(--radius-pill);
  padding: 6px 14px;
  width: fit-content;
  margin-bottom: 20px;

  .pulse-emerald-dot {
    width: 8px;
    height: 8px;
    border-radius: 50%;
    background: var(--accent-primary);
    box-shadow: 0 0 10px var(--accent-primary);
    animation: pulseGlow 2s infinite cubic-bezier(0.4, 0, 0.6, 1);
  }

  .availability-text {
    font-family: var(--font-mono);
    font-size: 0.72rem;
    font-weight: 700;
    letter-spacing: 0.06em;
    color: var(--accent-primary);
    text-transform: uppercase;
  }
}

@keyframes pulseGlow {
  0%, 100% {
    opacity: 1;
    transform: scale(1);
  }
  50% {
    opacity: 0.5;
    transform: scale(1.3);
  }
}

.contact-hero-title {
  font-family: var(--font-headline);
  font-size: clamp(2rem, 3.5vw, 2.6rem);
  font-weight: 800;
  color: var(--text-primary);
  line-height: 1.18;
  letter-spacing: -0.03em;
  margin: 0 0 16px;
}

.contact-hero-desc {
  font-family: var(--font-body);
  font-size: 0.98rem;
  line-height: 1.68;
  color: var(--text-secondary);
  margin: 0 0 32px;
}

/* Direct Channels Stack */
.direct-channels-stack {
  display: flex;
  flex-direction: column;
  gap: 14px;
  margin-bottom: 32px;
}

.channel-card {
  display: flex;
  align-items: center;
  gap: 14px;
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-md);
  padding: 14px 16px;
  transition: all 0.2s ease;

  &:hover {
    border-color: rgba(223, 38, 106, 0.3);
    background: var(--bg-surface-high);
  }

  .channel-icon-box {
    width: 38px;
    height: 38px;
    border-radius: var(--radius-sm);
    background: var(--accent-primary-container);
    color: var(--accent-primary);
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
  }

  .channel-info {
    display: flex;
    flex-direction: column;
    flex: 1;
    overflow: hidden;

    .channel-label {
      font-family: var(--font-mono);
      font-size: 0.68rem;
      font-weight: 700;
      color: var(--text-muted);
      letter-spacing: 0.05em;
    }

    .channel-value {
      font-family: var(--font-body);
      font-size: 0.9rem;
      color: var(--text-primary);
      text-decoration: none;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;

      &.email-link {
        font-family: var(--font-mono);
        color: var(--accent-primary);
        font-weight: 600;

        &:hover {
          text-decoration: underline;
        }
      }
    }
  }

  .channel-action-btn {
    background: transparent;
    border: 1px solid var(--border-subtle);
    border-radius: var(--radius-sm);
    color: var(--text-muted);
    padding: 6px;
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: all 0.2s ease;

    &:hover {
      color: var(--accent-primary);
      border-color: var(--accent-primary);
      background: var(--bg-surface-lowest);
    }
  }
}

/* Social Channels Pills */
.social-channels-group {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.social-pill-btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  color: var(--text-secondary);
  font-family: var(--font-headline);
  font-size: 0.8rem;
  font-weight: 600;
  padding: 8px 14px;
  border-radius: var(--radius-pill);
  text-decoration: none;
  transition: all 0.2s ease;

  .arrow-up {
    color: var(--text-muted);
    transition: transform 0.2s ease;
  }

  &:hover {
    color: var(--accent-primary);
    border-color: rgba(223, 38, 106, 0.4);
    background: var(--bg-surface-high);

    .arrow-up {
      transform: translate(2px, -2px);
      color: var(--accent-primary);
    }
  }

  &.cv-accent {
    border-color: rgba(99, 102, 241, 0.35);
    background: rgba(99, 102, 241, 0.08);
    color: #a5b4fc;

    &:hover {
      border-color: #a5b4fc;
      background: rgba(99, 102, 241, 0.16);
    }
  }
}

/* --------------------------------------------------------------------------
   RIGHT COLUMN: EDITORIAL CONTACT FORM CARD
   -------------------------------------------------------------------------- */
.contact-form-column {
  width: 100%;
}

.contact-form-card {
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-xl);
  padding: 36px 36px 32px;
  box-shadow: var(--shadow-card);
  position: relative;
  overflow: hidden;
}

.form-card-header {
  margin-bottom: 28px;

  .form-eyebrow {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    font-family: var(--font-mono);
    font-size: 0.72rem;
    font-weight: 700;
    color: var(--accent-primary);
    letter-spacing: 0.06em;
    text-transform: uppercase;
    margin-bottom: 8px;
  }

  .form-card-title {
    font-family: var(--font-headline);
    font-size: 1.65rem;
    font-weight: 800;
    letter-spacing: -0.02em;
    color: var(--text-primary);
    margin: 0;
  }
}

/* Form Fields */
.editorial-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.hidden-honeypot {
  display: none !important;
}

.form-dual-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 18px;
}

.field-container {
  display: flex;
  flex-direction: column;
  gap: 6px;

  .label-with-meta {
    display: flex;
    justify-content: space-between;
    align-items: center;

    .char-count {
      font-family: var(--font-mono);
      font-size: 0.7rem;
      color: var(--text-muted);
    }
  }

  .field-label {
    font-family: var(--font-headline);
    font-size: 0.82rem;
    font-weight: 600;
    color: var(--text-secondary);
  }

  .editorial-input,
  .editorial-textarea {
    background: var(--bg-surface-lowest);
    border: 1px solid var(--border-subtle);
    border-radius: var(--radius-md);
    padding: 12px 16px;
    font-family: var(--font-body);
    font-size: 0.92rem;
    color: var(--text-primary);
    outline: none;
    transition: all 0.2s ease;
    box-sizing: border-box;
    width: 100%;

    &::placeholder {
      color: var(--text-muted);
      opacity: 0.6;
    }

    &:focus {
      border-color: var(--accent-primary);
      box-shadow: 0 0 0 1px var(--accent-primary);
      background: var(--bg-surface-high);
    }

    &:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }
  }

  .editorial-textarea {
    resize: vertical;
    min-height: 140px;
    line-height: 1.6;
  }
}

/* Form Action & Submit Button */
.form-action-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  margin-top: 8px;
  padding-top: 14px;
  border-top: 1px solid var(--border-hairline);
}

.editorial-submit-btn {
  font-family: var(--font-headline);
  background: var(--accent-primary);
  color: #022c22;
  font-size: 0.92rem;
  font-weight: 700;
  border: none;
  border-radius: var(--radius-md);
  padding: 13px 28px;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 10px;
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);

  .btn-arrow {
    transition: transform 0.2s ease;
  }

  &:hover:not(:disabled) {
    background: var(--accent-primary-hover);
    box-shadow: 0 6px 20px rgba(223, 38, 106, 0.35);
    transform: translateY(-2px);

    .btn-arrow {
      transform: translateX(4px);
    }
  }

  &:disabled {
    opacity: 0.5;
    cursor: not-allowed;
    transform: none;
  }
}

.form-trust-indicator {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-family: var(--font-mono);
  font-size: 0.72rem;
  color: var(--text-muted);
}

/* Submission Alert States */
.submission-alert-banner {
  display: flex;
  align-items: center;
  gap: 16px;
  border-radius: var(--radius-lg);
  padding: 24px 20px;
  margin-bottom: 20px;

  &.success-banner {
    background: rgba(223, 38, 106, 0.1);
    border: 1px solid rgba(223, 38, 106, 0.3);

    .alert-icon-circle {
      width: 44px;
      height: 44px;
      border-radius: 50%;
      background: var(--accent-primary);
      color: #022c22;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }

    .alert-text-block {
      flex: 1;

      strong {
        display: block;
        font-family: var(--font-headline);
        font-size: 1.05rem;
        color: var(--text-primary);
        margin-bottom: 4px;
      }

      p {
        font-family: var(--font-body);
        font-size: 0.85rem;
        color: var(--text-secondary);
        margin: 0;
      }
    }

    .alert-reset-btn {
      background: var(--bg-surface-high);
      border: 1px solid var(--border-subtle);
      border-radius: var(--radius-sm);
      color: var(--accent-primary);
      font-family: var(--font-mono);
      font-size: 0.75rem;
      font-weight: 600;
      padding: 8px 14px;
      cursor: pointer;
      display: flex;
      align-items: center;
      gap: 6px;
      transition: all 0.2s ease;

      &:hover {
        background: var(--bg-surface-lowest);
        border-color: var(--accent-primary);
      }
    }
  }

  &.error-banner {
    background: rgba(239, 68, 68, 0.1);
    border: 1px solid rgba(239, 68, 68, 0.3);
    color: #fca5a5;

    .error-ico {
      color: #ef4444;
      flex-shrink: 0;
    }

    .alert-text-block {
      flex: 1;
      font-family: var(--font-body);
      font-size: 0.9rem;
    }

    .alert-close-btn {
      background: transparent;
      border: none;
      color: #fca5a5;
      cursor: pointer;
      padding: 4px;
    }
  }
}

.contact-map-section {
  margin-top: 64px;
  padding-top: 48px;
  border-top: 1px solid var(--border-hairline);
}

/* ==========================================================================
   3. RESPONSIVE BREAKPOINTS (Skill_AIGEN Mandate)
   ========================================================================== */
@media (max-width: 960px) {
  .contact-grid-layout {
    grid-template-columns: 1fr;
    gap: 40px;
  }
}

@media (max-width: 640px) {
  .portfolio-content {
    padding: 24px 16px 60px;
  }

  .contact-form-card {
    padding: 24px 18px 20px;
  }

  .form-dual-row {
    grid-template-columns: 1fr;
    gap: 16px;
  }

  .form-action-row {
    flex-direction: column;
    align-items: stretch;
    gap: 12px;
  }

  .editorial-submit-btn {
    justify-content: center;
    width: 100%;
  }

  .form-trust-indicator {
    justify-content: center;
  }
}
</style>

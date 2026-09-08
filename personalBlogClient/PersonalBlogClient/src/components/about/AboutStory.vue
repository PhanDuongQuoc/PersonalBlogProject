<template>
  <section class="about-story-section">
    <div class="story-header">
      <span class="section-eyebrow">{{ text.myStoryTitle }}</span>
      <h2 class="section-heading">{{ text.myStorySubtitle }}</h2>
    </div>

    <div class="story-grid">
      <div class="story-card main-narrative">
        <div class="card-icon-badge">
          <q-icon name="auto_stories" size="24px" />
        </div>
        <p class="story-text">
          {{ story || defaultStory }}
        </p>
      </div>

      <div class="story-side-cards">
        <div class="side-card">
          <div class="side-card-header">
            <q-icon name="code" size="20px" class="card-icon" />
            <h3>Clean Architecture & Code Quality</h3>
          </div>
          <p>
            {{ locale === 'vi' 
              ? 'Tập trung vào kiến trúc sạch, dễ bảo trì, tuân thủ nguyên lý SOLID và tối ưu hóa hiệu năng render cũng như database.' 
              : 'Focused on maintainable architecture, SOLID principles, high-performance rendering and database query optimization.' }}
          </p>
        </div>

        <div class="side-card">
          <div class="side-card-header">
            <q-icon name="devices" size="20px" class="card-icon" />
            <h3>User Experience & Modern Design</h3>
          </div>
          <p>
            {{ locale === 'vi'
              ? 'Tạo nên giao diện hiện đại, trực quan, hỗ trợ đa thiết bị và chuyển đổi mượt mà giữa Dark và Light mode.'
              : 'Crafting modern, intuitive, responsive user interfaces with seamless dark and light mode adaptability.' }}
          </p>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';

const props = defineProps<{ story: string | null }>();
const { locale, text } = usePortfolioLocale();

const defaultStory = computed(() => locale.value === 'vi'
  ? 'Mình bắt đầu hành trình lập trình từ niềm say mê sáng tạo các giao diện web trực quan và logic backend mạnh mẽ. Với kinh nghiệm làm việc cùng hệ sinh thái Vue.js và ASP.NET Core, mình luôn hướng đến việc viết mã nguồn sạch (clean code), tối ưu hóa trải nghiệm người dùng và xây dựng kiến trúc hệ thống bền vững, dễ mở rộng.'
  : 'My programming journey began from the passion for crafting intuitive web interfaces and resilient backend systems. Specializing in the Vue.js and ASP.NET Core ecosystem, I focus on clean code, seamless UX, and scalable software architectures.');
</script>

<style scoped lang="scss">
.about-story-section {
  border-top: 1px solid #202b4a;
  padding: 74px 78px;
}

.story-header {
  margin-bottom: 34px;
}

.section-eyebrow {
  color: #46e0af;
  font-size: 0.72rem;
  font-weight: 800;
  letter-spacing: 0.13em;
  text-transform: uppercase;
}

.section-heading {
  color: #f1f4ff;
  font-size: clamp(1.8rem, 3.5vw, 2.6rem);
  font-weight: 900;
  letter-spacing: -0.05em;
  line-height: 1.15;
  margin: 10px 0 0;
}

.story-grid {
  display: grid;
  grid-template-columns: 1.3fr 1fr;
  gap: 24px;
  align-items: stretch;
}

.story-card {
  background: #151f37;
  border: 1px solid #26324e;
  border-radius: 4px;
  padding: 32px;
  position: relative;
}

.card-icon-badge {
  align-items: center;
  background: rgba(70, 224, 175, 0.12);
  border-radius: 8px;
  color: #46e0af;
  display: inline-flex;
  height: 44px;
  justify-content: center;
  margin-bottom: 20px;
  width: 44px;
}

.story-text {
  color: #d2e0f2;
  font-size: 1.05rem;
  line-height: 1.8;
  margin: 0;
  white-space: pre-line;
}

.story-side-cards {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.side-card {
  background: #0f182e;
  border: 1px solid #22304d;
  border-radius: 4px;
  display: flex;
  flex: 1;
  flex-direction: column;
  justify-content: center;
  padding: 24px 26px;
  transition: transform 0.2s ease, border-color 0.2s ease;
}

.side-card:hover {
  border-color: #38517c;
  transform: translateY(-2px);
}

.side-card-header {
  align-items: center;
  display: flex;
  gap: 12px;
  margin-bottom: 10px;
}

.card-icon {
  color: #35d6ff;
}

.side-card h3 {
  color: #f1f4ff;
  font-size: 1.05rem;
  font-weight: 800;
  letter-spacing: -0.02em;
  margin: 0;
}

.side-card p {
  color: #9db3ca;
  font-size: 0.88rem;
  line-height: 1.6;
  margin: 0;
}

@media (max-width: 860px) {
  .about-story-section {
    padding: 58px 24px;
  }

  .story-grid {
    grid-template-columns: 1fr;
  }
}
</style>

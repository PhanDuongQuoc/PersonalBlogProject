<template>
  <section class="about-timeline-section">
    <!-- Work Experience Section -->
    <div class="timeline-block">
      <div class="block-header">
        <span class="section-eyebrow">{{ text.workExperienceTitle }}</span>
        <h2 class="section-heading">{{ text.workExperienceSubtitle }}</h2>
      </div>

      <div v-if="experiences.length" class="timeline-track">
        <div
          v-for="exp in experiences"
          :key="exp.id"
          class="timeline-item"
        >
          <div class="item-bullet" :class="{ 'bullet-active': exp.isCurrent }"></div>
          <div class="timeline-card">
            <div class="card-header-row">
              <div>
                <h3 class="item-role">{{ exp.role }}</h3>
                <p class="item-company">
                  <q-icon name="business" size="16px" />
                  <strong>{{ exp.company }}</strong>
                  <span v-if="exp.location">· {{ exp.location }}</span>
                </p>
              </div>

              <div class="time-badge" :class="{ 'time-current': exp.isCurrent }">
                <q-icon name="calendar_today" size="13px" />
                <span>{{ exp.startDate }} - {{ exp.isCurrent ? text.present : (exp.endDate || text.present) }}</span>
              </div>
            </div>

            <p v-if="exp.description" class="item-description">
              {{ exp.description }}
            </p>

            <div v-if="exp.technologies" class="tech-tags-row">
              <span class="tech-tag-label">{{ text.technologiesUsed }}:</span>
              <span
                v-for="tech in splitTechs(exp.technologies)"
                :key="tech"
                class="tech-tag"
              >
                {{ tech }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <div v-else class="empty-timeline">
        <p>Chưa có dữ liệu kinh nghiệm làm việc.</p>
      </div>
    </div>

    <!-- Education & Certifications Section -->
    <div class="timeline-block education-block">
      <div class="block-header">
        <span class="section-eyebrow">{{ text.educationTitle }}</span>
        <h2 class="section-heading">{{ text.educationSubtitle }}</h2>
      </div>

      <div v-if="educations.length" class="timeline-track">
        <div
          v-for="edu in educations"
          :key="edu.id"
          class="timeline-item"
        >
          <div class="item-bullet bullet-edu"></div>
          <div class="timeline-card">
            <div class="card-header-row">
              <div>
                <h3 class="item-role">{{ edu.degree }}</h3>
                <p class="item-company">
                  <q-icon name="school" size="16px" />
                  <strong>{{ edu.institution }}</strong>
                </p>
              </div>

              <div v-if="edu.startYear || edu.endYear" class="time-badge">
                <q-icon name="date_range" size="13px" />
                <span>{{ edu.startYear || '' }} - {{ edu.endYear || text.present }}</span>
              </div>
            </div>

            <p v-if="edu.description" class="item-description">
              {{ edu.description }}
            </p>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
import type { PublicAboutEducation, PublicAboutExperience } from '@/types/public-about';

defineProps<{
  experiences: PublicAboutExperience[];
  educations: PublicAboutEducation[];
}>();

const { text } = usePortfolioLocale();

const splitTechs = (techString: string): string[] => {
  return techString
    .split(',')
    .map((t) => t.trim())
    .filter((t) => t.length > 0);
};
</script>

<style scoped lang="scss">
.about-timeline-section {
  border-top: 1px solid #202b4a;
  padding: 74px 78px;
}

.timeline-block + .timeline-block {
  margin-top: 70px;
  padding-top: 60px;
  border-top: 1px dashed #202b4a;
}

.block-header {
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

.timeline-track {
  border-left: 2px solid #1c2b4d;
  display: flex;
  flex-direction: column;
  gap: 28px;
  margin-left: 12px;
  padding-left: 28px;
  position: relative;
}

.timeline-item {
  position: relative;
}

.item-bullet {
  background: #081126;
  border: 3px solid #35d6ff;
  border-radius: 50%;
  height: 15px;
  left: -37px;
  position: absolute;
  top: 18px;
  width: 15px;
}

.bullet-active {
  background: #46e0af;
  border-color: #46e0af;
  box-shadow: 0 0 10px rgba(70, 224, 175, 0.7);
}

.bullet-edu {
  border-color: #a78bfa;
}

.timeline-card {
  background: #151f37;
  border: 1px solid #26324e;
  border-radius: 4px;
  padding: 24px 28px;
  transition: transform 0.2s ease, border-color 0.2s ease;
}

.timeline-card:hover {
  border-color: #38517c;
  transform: translateY(-2px);
}

.card-header-row {
  align-items: flex-start;
  display: flex;
  flex-wrap: wrap;
  gap: 14px;
  justify-content: space-between;
}

.item-role {
  color: #f1f4ff;
  font-size: 1.15rem;
  font-weight: 800;
  letter-spacing: -0.02em;
  margin: 0 0 6px;
}

.item-company {
  align-items: center;
  color: #a2b7cd;
  display: flex;
  font-size: 0.86rem;
  gap: 6px;
  margin: 0;
}

.item-company .q-icon {
  color: #35d6ff;
}

.item-company strong {
  color: #d8e5f7;
}

.time-badge {
  align-items: center;
  background: #0e172d;
  border: 1px solid #243557;
  border-radius: 4px;
  color: #a4b9cf;
  display: inline-flex;
  font-size: 0.75rem;
  font-weight: 700;
  gap: 6px;
  padding: 6px 12px;
}

.time-current {
  background: rgba(70, 224, 175, 0.12);
  border-color: rgba(70, 224, 175, 0.35);
  color: #46e0af;
}

.item-description {
  color: #b9cce0;
  font-size: 0.92rem;
  line-height: 1.7;
  margin: 14px 0 0;
  white-space: pre-line;
}

.tech-tags-row {
  align-items: center;
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-top: 16px;
  padding-top: 14px;
  border-top: 1px solid #1f2a44;
}

.tech-tag-label {
  color: #7b94ad;
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.05em;
  text-transform: uppercase;
}

.tech-tag {
  background: #0f1a30;
  border: 1px solid #233454;
  border-radius: 3px;
  color: #c4d7ed;
  font-size: 0.75rem;
  font-weight: 600;
  padding: 3px 8px;
}

.empty-timeline {
  background: #151f37;
  border: 1px dashed #355071;
  border-radius: 4px;
  color: #a9c1d3;
  padding: 24px;
}

@media (max-width: 760px) {
  .about-timeline-section {
    padding: 58px 24px;
  }

  .timeline-track {
    margin-left: 6px;
    padding-left: 20px;
  }

  .item-bullet {
    left: -29px;
  }

  .card-header-row {
    flex-direction: column;
  }
}
</style>

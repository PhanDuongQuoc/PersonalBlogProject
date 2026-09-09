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
  border-bottom: 1px solid var(--border-hairline);
  padding: 64px 0;
}

.timeline-block + .timeline-block {
  margin-top: 64px;
  padding-top: 56px;
  border-top: 1px dashed var(--border-hairline);
}

.block-header {
  margin-bottom: 30px;
}

.section-eyebrow {
  font-family: var(--font-mono);
  color: var(--accent-primary);
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.12em;
  text-transform: uppercase;
}

.section-heading {
  font-family: var(--font-headline);
  color: var(--text-primary);
  font-size: clamp(1.8rem, 3.5vw, 2.5rem);
  font-weight: 800;
  letter-spacing: -0.03em;
  line-height: 1.15;
  margin: 8px 0 0;
}

.timeline-track {
  border-left: 2px solid var(--border-hairline);
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
  background: var(--bg-canvas);
  border: 3px solid var(--accent-primary);
  border-radius: 50%;
  height: 14px;
  left: -36px;
  position: absolute;
  top: 18px;
  width: 14px;
}

.bullet-active {
  background: var(--accent-primary);
  box-shadow: 0 0 10px rgba(16, 185, 129, 0.7);
}

.bullet-edu {
  border-color: var(--accent-secondary);
}

.timeline-card {
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-lg);
  padding: 24px 28px;
  box-shadow: var(--shadow-card);
  transition: transform 0.2s ease, border-color 0.2s ease;

  &:hover {
    border-color: rgba(16, 185, 129, 0.35);
    transform: translateY(-2px);
  }
}

.card-header-row {
  align-items: flex-start;
  display: flex;
  flex-wrap: wrap;
  gap: 14px;
  justify-content: space-between;
}

.item-role {
  font-family: var(--font-headline);
  color: var(--text-primary);
  font-size: 1.15rem;
  font-weight: 700;
  letter-spacing: -0.015em;
  margin: 0 0 4px;
}

.item-company {
  align-items: center;
  color: var(--text-secondary);
  display: flex;
  font-size: 0.86rem;
  gap: 6px;
  margin: 0;

  .q-icon {
    color: var(--accent-primary);
  }

  strong {
    color: var(--text-primary);
  }
}

.time-badge {
  font-family: var(--font-mono);
  align-items: center;
  background: var(--bg-surface-high);
  border: 1px solid var(--border-subtle);
  border-radius: var(--radius-sm);
  color: var(--text-secondary);
  display: inline-flex;
  font-size: 0.74rem;
  font-weight: 600;
  gap: 6px;
  padding: 5px 12px;
}

.time-current {
  background: var(--accent-primary-container);
  border-color: rgba(16, 185, 129, 0.35);
  color: var(--accent-primary);
}

.item-description {
  font-family: var(--font-body);
  color: var(--text-secondary);
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
  border-top: 1px solid var(--border-hairline);
}

.tech-tag-label {
  font-family: var(--font-mono);
  color: var(--text-muted);
  font-size: 0.7rem;
  font-weight: 700;
  letter-spacing: 0.05em;
  text-transform: uppercase;
}

.tech-tag {
  font-family: var(--font-mono);
  background: var(--accent-secondary-container);
  border: 1px solid var(--accent-secondary-border);
  border-radius: var(--radius-sm);
  color: var(--accent-secondary-text);
  font-size: 0.74rem;
  font-weight: 600;
  padding: 2px 8px;
}

.empty-timeline {
  background: var(--bg-surface-low);
  border: 1px dashed var(--border-subtle);
  border-radius: var(--radius-md);
  color: var(--text-muted);
  padding: 24px;
}

@media (max-width: 760px) {
  .timeline-track {
    margin-left: 6px;
    padding-left: 20px;
  }

  .item-bullet {
    left: -28px;
  }

  .card-header-row {
    flex-direction: column;
  }
}
</style>

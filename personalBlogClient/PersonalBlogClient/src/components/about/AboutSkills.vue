<template>
  <section class="about-skills-section">
    <div class="skills-header">
      <div class="header-text">
        <span class="section-eyebrow">{{ text.techStackTitle }}</span>
        <h2 class="section-heading">{{ text.techStackSubtitle }}</h2>
      </div>

      <!-- Category Filter Tabs -->
      <div class="category-tabs">
        <button
          type="button"
          class="tab-btn"
          :class="{ active: selectedCategory === 'ALL' }"
          @click="selectedCategory = 'ALL'"
        >
          {{ text.allCategories }}
        </button>
        <button
          v-for="cat in availableCategories"
          :key="cat"
          type="button"
          class="tab-btn"
          :class="{ active: selectedCategory === cat }"
          @click="selectedCategory = cat"
        >
          {{ cat }}
        </button>
      </div>
    </div>

    <!-- Skills Grid -->
    <div v-if="filteredSkills.length" class="skills-grid">
      <div
        v-for="skill in filteredSkills"
        :key="skill.id"
        class="skill-card"
      >
        <div class="skill-top">
          <div class="skill-name-row">
            <q-icon
              :name="getSkillIcon(skill)"
              size="20px"
              class="skill-icon"
            />
            <span class="skill-title">{{ skill.name }}</span>
          </div>
          <span class="skill-percent">{{ skill.proficiency }}%</span>
        </div>

        <div class="skill-category-tag">{{ skill.category }}</div>

        <!-- Progress Track -->
        <div class="skill-progress-bg">
          <div
            class="skill-progress-fill"
            :style="{ width: `${skill.proficiency}%` }"
          ></div>
        </div>
      </div>
    </div>

    <div v-else class="empty-skills">
      <q-icon name="build" size="28px" />
      <p>Chưa có thông tin kỹ năng nào.</p>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
import type { PublicAboutSkill } from '@/types/public-about';

const props = defineProps<{ skills: PublicAboutSkill[] }>();
const { text } = usePortfolioLocale();

const selectedCategory = ref<string>('ALL');

const availableCategories = computed(() => {
  const cats = new Set<string>();
  props.skills.forEach((s) => {
    if (s.category) cats.add(s.category);
  });
  return Array.from(cats);
});

const filteredSkills = computed(() => {
  if (selectedCategory.value === 'ALL') {
    return props.skills;
  }
  return props.skills.filter((s) => s.category === selectedCategory.value);
});

const getSkillIcon = (skill: PublicAboutSkill): string => {
  if (skill.icon && skill.icon.trim()) return skill.icon;
  const name = skill.name.toLowerCase();
  if (name.includes('vue')) return 'code';
  if (name.includes('script') || name.includes('js')) return 'javascript';
  if (name.includes('net') || name.includes('c#')) return 'memory';
  if (name.includes('sql') || name.includes('data')) return 'storage';
  if (name.includes('docker') || name.includes('deploy')) return 'cloud';
  if (name.includes('git')) return 'source';
  return 'extension';
};
</script>

<style scoped lang="scss">
.about-skills-section {
  border-bottom: 1px solid var(--border-hairline);
  padding: 64px 0;
}

.skills-header {
  display: flex;
  flex-direction: column;
  gap: 18px;
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

.category-tabs {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-top: 6px;
}

.tab-btn {
  font-family: var(--font-headline);
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-md);
  color: var(--text-secondary);
  cursor: pointer;
  font-size: 0.8rem;
  font-weight: 600;
  padding: 6px 14px;
  transition: all 0.2s ease;

  &:hover {
    background: var(--bg-surface-high);
    color: var(--text-primary);
  }

  &.active {
    background: var(--accent-primary);
    border-color: var(--accent-primary);
    color: var(--accent-on-primary);
    font-weight: 700;
  }
}

.skills-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 18px;
}

.skill-card {
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-lg);
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding: 20px 22px;
  box-shadow: var(--shadow-card);
  transition: transform 0.2s ease, border-color 0.2s ease;

  &:hover {
    border-color: rgba(16, 185, 129, 0.35);
    transform: translateY(-2px);
  }
}

.skill-top {
  align-items: center;
  display: flex;
  justify-content: space-between;
}

.skill-name-row {
  align-items: center;
  display: flex;
  gap: 10px;
}

.skill-icon {
  color: var(--accent-primary);
}

.skill-title {
  font-family: var(--font-headline);
  color: var(--text-primary);
  font-size: 0.95rem;
  font-weight: 700;
}

.skill-percent {
  font-family: var(--font-mono);
  color: var(--accent-primary);
  font-size: 0.85rem;
  font-weight: 800;
}

.skill-category-tag {
  font-family: var(--font-mono);
  color: var(--text-muted);
  font-size: 0.7rem;
  font-weight: 600;
  letter-spacing: 0.05em;
  margin-top: -4px;
  text-transform: uppercase;
}

.skill-progress-bg {
  background: var(--bg-surface-high);
  border-radius: var(--radius-pill);
  height: 6px;
  overflow: hidden;
  position: relative;
  width: 100%;
}

.skill-progress-fill {
  background: var(--accent-primary);
  border-radius: var(--radius-pill);
  height: 100%;
  transition: width 0.6s cubic-bezier(0.2, 0.8, 0.2, 1);
}

.empty-skills {
  align-items: center;
  background: var(--bg-surface-low);
  border: 1px dashed var(--border-subtle);
  border-radius: var(--radius-md);
  color: var(--text-muted);
  display: flex;
  gap: 14px;
  padding: 28px;
}

@media (max-width: 760px) {
  .skills-grid {
    grid-template-columns: 1fr;
  }
}
</style>

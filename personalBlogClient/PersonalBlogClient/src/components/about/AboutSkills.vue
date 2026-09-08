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
              size="22px"
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
  border-top: 1px solid #202b4a;
  padding: 74px 78px;
}

.skills-header {
  display: flex;
  flex-direction: column;
  gap: 20px;
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

.category-tabs {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-top: 8px;
}

.tab-btn {
  background: #111a33;
  border: 1px solid #26385a;
  border-radius: 4px;
  color: #9eb4cc;
  cursor: pointer;
  font-family: inherit;
  font-size: 0.78rem;
  font-weight: 700;
  padding: 8px 16px;
  transition: all 0.2s ease;
}

.tab-btn:hover {
  background: #182647;
  border-color: #3f5d8f;
  color: #e2ecfa;
}

.tab-btn.active {
  background: #46e0af;
  border-color: #46e0af;
  color: #071126;
}

.skills-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 18px;
}

.skill-card {
  background: #151f37;
  border: 1px solid #26324e;
  border-radius: 4px;
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding: 20px 22px;
  transition: transform 0.2s ease, border-color 0.2s ease;
}

.skill-card:hover {
  border-color: #38517c;
  transform: translateY(-2px);
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
  color: #35d6ff;
}

.skill-title {
  color: #f1f4ff;
  font-size: 0.95rem;
  font-weight: 800;
  letter-spacing: -0.01em;
}

.skill-percent {
  color: #46e0af;
  font-size: 0.85rem;
  font-weight: 800;
}

.skill-category-tag {
  color: #7b94ad;
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.06em;
  margin-top: -4px;
  text-transform: uppercase;
}

.skill-progress-bg {
  background: #0d152a;
  border-radius: 4px;
  height: 6px;
  overflow: hidden;
  position: relative;
  width: 100%;
}

.skill-progress-fill {
  background: linear-gradient(90deg, #35d6ff, #46e0af);
  border-radius: 4px;
  height: 100%;
  transition: width 0.6s cubic-bezier(0.2, 0.8, 0.2, 1);
}

.empty-skills {
  align-items: center;
  background: #151f37;
  border: 1px dashed #355071;
  border-radius: 4px;
  color: #a9c1d3;
  display: flex;
  gap: 14px;
  padding: 28px;
}

@media (max-width: 760px) {
  .about-skills-section {
    padding: 58px 24px;
  }

  .skills-grid {
    grid-template-columns: 1fr;
  }
}
</style>

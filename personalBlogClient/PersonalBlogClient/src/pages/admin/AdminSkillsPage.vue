<template>
  <q-page class="admin-skills-page">
    <!-- 1. Stats Quick Overview Cards -->
    <section class="skills-stats-grid">
      <!-- Card: Total Skills -->
      <div class="stat-card card-rose">
        <div class="stat-icon-box icon-rose">
          <q-icon name="fa-solid fa-code" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.totalSkills }}</div>
          <div class="stat-label">KỸ NĂNG CHUYÊN MÔN</div>
        </div>
      </div>

      <!-- Card: Total Experiences -->
      <div class="stat-card card-emerald">
        <div class="stat-icon-box icon-emerald">
          <q-icon name="fa-solid fa-briefcase" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.totalExperiences }}</div>
          <div class="stat-label">KINH NGHIỆM LÀM VIỆC</div>
        </div>
      </div>

      <!-- Card: Total Educations -->
      <div class="stat-card card-amber">
        <div class="stat-icon-box icon-amber">
          <q-icon name="fa-solid fa-graduation-cap" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.totalEducations }}</div>
          <div class="stat-label">HỌC VẤN & BẰNG CẤP</div>
        </div>
      </div>

      <!-- Card: High Proficiency Skills -->
      <div class="stat-card card-indigo">
        <div class="stat-icon-box icon-indigo">
          <q-icon name="fa-solid fa-award" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.highProficiencySkillsCount }}</div>
          <div class="stat-label">THUẦN THỤC (>= 90%)</div>
        </div>
      </div>
    </section>

    <!-- 2. Dynamic Section Content based on Active Tab -->

    <!-- TAB 1: TECHNICAL SKILLS -->
    <section v-if="activeTab === 'skills'" class="tab-table-section">
      <AdminDataTable
        :items="filteredSkills"
        :columns="skillColumns"
        :loading="loadingSkills"
        v-model:search="skillSearch"
        search-placeholder="Tìm kiếm kỹ năng, nhóm..."
        :pagination="skillPagination"
        empty-title="Chưa có kỹ năng nào"
        empty-message="Chưa có kỹ năng nào phù hợp với từ khóa tìm kiếm."
        @page-change="(p) => skillPage = p"
      >
        <!-- Horizontal Tabs Group -->
        <template #prepend-search>
          <div class="skills-tabs-pill-bar">
            <button
              type="button"
              class="tab-pill-btn active"
              @click="activeTab = 'skills'"
            >
              <q-icon name="fa-solid fa-code" size="12px" />
              <span>Kỹ năng chuyên môn ({{ skills.length }})</span>
            </button>
            <button
              type="button"
              class="tab-pill-btn"
              @click="activeTab = 'experiences'"
            >
              <q-icon name="fa-solid fa-briefcase" size="12px" />
              <span>Kinh nghiệm làm việc ({{ experiences.length }})</span>
            </button>
            <button
              type="button"
              class="tab-pill-btn"
              @click="activeTab = 'educations'"
            >
              <q-icon name="fa-solid fa-graduation-cap" size="12px" />
              <span>Học vấn & Bằng cấp ({{ educations.length }})</span>
            </button>
          </div>
        </template>

        <template #filters>
          <button type="button" class="btn-refresh" title="Tải lại dữ liệu" @click="fetchSkills">
            <q-icon name="fa-solid fa-rotate-right" size="12px" :class="{ 'fa-spin': loadingSkills }" />
          </button>
        </template>

        <template #actions>
          <button type="button" class="btn-primary-rose" @click="openCreateSkillDialog">
            <q-icon name="fa-solid fa-plus" size="13px" />
            <span>Thêm kỹ năng mới</span>
          </button>
        </template>

        <!-- Custom Cell: Name with Icon -->
        <template #body-cell-name="{ row }">
          <div class="skill-name-cell">
            <div class="skill-mini-icon">
              <q-icon :name="resolveSkillIcon(row)" size="14px" />
            </div>
            <span class="skill-name-text">{{ row.name }}</span>
          </div>
        </template>

        <!-- Custom Cell: Category -->
        <template #body-cell-category="{ row }">
          <span class="category-pill-badge" :class="`cat-${row.category.toLowerCase().replace(/[^a-z]/g, '')}`">
            {{ row.category }}
          </span>
        </template>

        <!-- Custom Cell: Proficiency -->
        <template #body-cell-proficiency="{ row }">
          <div class="proficiency-cell">
            <div class="progress-track">
              <div class="progress-bar" :style="{ width: `${row.proficiency}%` }"></div>
            </div>
            <span class="proficiency-num">{{ row.proficiency }}%</span>
          </div>
        </template>

        <!-- Custom Cell: Display Order -->
        <template #body-cell-displayOrder="{ row }">
          <span class="order-badge">#{{ row.displayOrder }}</span>
        </template>

        <!-- Custom Cell: Actions -->
        <template #body-cell-actions="{ row }">
          <div class="table-actions-cell">
            <button
              type="button"
              class="table-action-btn btn-edit"
              title="Chỉnh sửa kỹ năng"
              @click="openEditSkillDialog(row)"
            >
              <q-icon name="fa-solid fa-pen-to-square" size="12px" />
            </button>
            <button
              type="button"
              class="table-action-btn btn-delete"
              title="Xóa kỹ năng"
              @click="confirmDeleteSkill(row)"
            >
              <q-icon name="fa-solid fa-trash-can" size="12px" />
            </button>
          </div>
        </template>
      </AdminDataTable>
    </section>

    <!-- TAB 2: WORK EXPERIENCES -->
    <section v-else-if="activeTab === 'experiences'" class="tab-table-section">
      <AdminDataTable
        :items="filteredExperiences"
        :columns="experienceColumns"
        :loading="loadingExperiences"
        v-model:search="experienceSearch"
        search-placeholder="Tìm kiếm kinh nghiệm, công ty, vị trí..."
        :pagination="experiencePagination"
        empty-title="Chưa có kinh nghiệm nào"
        empty-message="Chưa có kinh nghiệm nào phù hợp với từ khóa tìm kiếm."
        @page-change="(p) => experiencePage = p"
      >
        <!-- Horizontal Tabs Group -->
        <template #prepend-search>
          <div class="skills-tabs-pill-bar">
            <button
              type="button"
              class="tab-pill-btn"
              @click="activeTab = 'skills'"
            >
              <q-icon name="fa-solid fa-code" size="12px" />
              <span>Kỹ năng chuyên môn ({{ skills.length }})</span>
            </button>
            <button
              type="button"
              class="tab-pill-btn active"
              @click="activeTab = 'experiences'"
            >
              <q-icon name="fa-solid fa-briefcase" size="12px" />
              <span>Kinh nghiệm làm việc ({{ experiences.length }})</span>
            </button>
            <button
              type="button"
              class="tab-pill-btn"
              @click="activeTab = 'educations'"
            >
              <q-icon name="fa-solid fa-graduation-cap" size="12px" />
              <span>Học vấn & Bằng cấp ({{ educations.length }})</span>
            </button>
          </div>
        </template>

        <template #filters>
          <button type="button" class="btn-refresh" title="Tải lại dữ liệu" @click="fetchExperiences">
            <q-icon name="fa-solid fa-rotate-right" size="12px" :class="{ 'fa-spin': loadingExperiences }" />
          </button>
        </template>

        <template #actions>
          <button type="button" class="btn-primary-rose" @click="openCreateExperienceDialog">
            <q-icon name="fa-solid fa-plus" size="13px" />
            <span>Thêm kinh nghiệm mới</span>
          </button>
        </template>

        <!-- Custom Cell: Role & Company -->
        <template #body-cell-role="{ row }">
          <div class="role-company-cell">
            <div class="role-text">{{ row.role }}</div>
            <div class="company-sub">
              <span class="company-name">{{ row.company }}</span>
              <span v-if="row.location" class="meta-dot">·</span>
              <span v-if="row.location" class="location-text">{{ row.location }}</span>
            </div>
          </div>
        </template>

        <!-- Custom Cell: Duration -->
        <template #body-cell-duration="{ row }">
          <div class="duration-cell">
            <div class="date-range">
              {{ row.startDate }} — {{ row.isCurrent ? 'Hiện tại' : (row.endDate || '—') }}
            </div>
            <span v-if="row.isCurrent" class="current-badge">
              <span class="pulse-dot"></span>
              Đang làm việc
            </span>
          </div>
        </template>

        <!-- Custom Cell: Technologies & Description -->
        <template #body-cell-technologies="{ row }">
          <div class="tech-desc-cell">
            <div v-if="row.technologies" class="exp-tech-tags">
              <span
                v-for="(tech, tIdx) in row.technologies.split(',')"
                :key="tIdx"
                class="tech-tag-chip"
              >
                #{{ tech.trim() }}
              </span>
            </div>
            <div v-if="row.description" class="exp-desc-text" :title="row.description">
              {{ row.description }}
            </div>
          </div>
        </template>

        <!-- Custom Cell: Actions -->
        <template #body-cell-actions="{ row }">
          <div class="table-actions-cell">
            <button
              type="button"
              class="table-action-btn btn-edit"
              title="Chỉnh sửa kinh nghiệm"
              @click="openEditExperienceDialog(row)"
            >
              <q-icon name="fa-solid fa-pen-to-square" size="12px" />
            </button>
            <button
              type="button"
              class="table-action-btn btn-delete"
              title="Xóa kinh nghiệm"
              @click="confirmDeleteExperience(row)"
            >
              <q-icon name="fa-solid fa-trash-can" size="12px" />
            </button>
          </div>
        </template>
      </AdminDataTable>
    </section>

    <!-- TAB 3: EDUCATION & DEGREES -->
    <section v-else class="tab-table-section">
      <AdminDataTable
        :items="filteredEducations"
        :columns="educationColumns"
        :loading="loadingEducations"
        v-model:search="educationSearch"
        search-placeholder="Tìm kiếm trường học, văn bằng..."
        :pagination="educationPagination"
        empty-title="Chưa có học vấn nào"
        empty-message="Chưa có thông tin học vấn nào phù hợp với từ khóa tìm kiếm."
        @page-change="(p) => educationPage = p"
      >
        <!-- Horizontal Tabs Group -->
        <template #prepend-search>
          <div class="skills-tabs-pill-bar">
            <button
              type="button"
              class="tab-pill-btn"
              @click="activeTab = 'skills'"
            >
              <q-icon name="fa-solid fa-code" size="12px" />
              <span>Kỹ năng chuyên môn ({{ skills.length }})</span>
            </button>
            <button
              type="button"
              class="tab-pill-btn"
              @click="activeTab = 'experiences'"
            >
              <q-icon name="fa-solid fa-briefcase" size="12px" />
              <span>Kinh nghiệm làm việc ({{ experiences.length }})</span>
            </button>
            <button
              type="button"
              class="tab-pill-btn active"
              @click="activeTab = 'educations'"
            >
              <q-icon name="fa-solid fa-graduation-cap" size="12px" />
              <span>Học vấn & Bằng cấp ({{ educations.length }})</span>
            </button>
          </div>
        </template>

        <template #filters>
          <button type="button" class="btn-refresh" title="Tải lại dữ liệu" @click="fetchEducations">
            <q-icon name="fa-solid fa-rotate-right" size="12px" :class="{ 'fa-spin': loadingEducations }" />
          </button>
        </template>

        <template #actions>
          <button type="button" class="btn-primary-rose" @click="openCreateEducationDialog">
            <q-icon name="fa-solid fa-plus" size="13px" />
            <span>Thêm học vấn mới</span>
          </button>
        </template>

        <!-- Custom Cell: Degree & Institution -->
        <template #body-cell-degree="{ row }">
          <div class="degree-inst-cell">
            <div class="degree-text">{{ row.degree }}</div>
            <div class="inst-sub">
              <q-icon name="fa-solid fa-building-columns" size="10px" class="q-mr-xs text-rose" />
              <span>{{ row.institution }}</span>
            </div>
          </div>
        </template>

        <!-- Custom Cell: Years -->
        <template #body-cell-years="{ row }">
          <div class="years-cell">
            {{ row.startYear || '—' }} — {{ row.endYear || '—' }}
          </div>
        </template>

        <!-- Custom Cell: Description -->
        <template #body-cell-description="{ row }">
          <div class="edu-desc-text" :title="row.description || 'Chưa có mô tả'">
            {{ row.description || '—' }}
          </div>
        </template>

        <!-- Custom Cell: Actions -->
        <template #body-cell-actions="{ row }">
          <div class="table-actions-cell">
            <button
              type="button"
              class="table-action-btn btn-edit"
              title="Chỉnh sửa học vấn"
              @click="openEditEducationDialog(row)"
            >
              <q-icon name="fa-solid fa-pen-to-square" size="12px" />
            </button>
            <button
              type="button"
              class="table-action-btn btn-delete"
              title="Xóa học vấn"
              @click="confirmDeleteEducation(row)"
            >
              <q-icon name="fa-solid fa-trash-can" size="12px" />
            </button>
          </div>
        </template>
      </AdminDataTable>
    </section>

    <!-- 4. Modals -->
    <AdminSkillEditorDialog
      v-model="skillDialogOpen"
      :skill="selectedSkill"
      @saved="onSkillSaved"
    />

    <AdminExperienceEditorDialog
      v-model="experienceDialogOpen"
      :experience="selectedExperience"
      @saved="onExperienceSaved"
    />

    <AdminEducationEditorDialog
      v-model="educationDialogOpen"
      :education="selectedEducation"
      @saved="onEducationSaved"
    />
  </q-page>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from "vue";
import type { ColumnDef, TablePagination } from "@/components/admin/AdminDataTable.vue";
import AdminDataTable from "@/components/admin/AdminDataTable.vue";
import AdminSkillEditorDialog from "@/components/admin/AdminSkillEditorDialog.vue";
import AdminExperienceEditorDialog from "@/components/admin/AdminExperienceEditorDialog.vue";
import AdminEducationEditorDialog from "@/components/admin/AdminEducationEditorDialog.vue";
import type {
  AdminSkill,
  AdminExperience,
  AdminEducation,
  AdminSkillsOverviewStats
} from "@/types/admin-profile";
import { adminProfileService } from "@/services/admin-profile.service";
import { swalConfirm, swalSuccess, swalError } from "@/utils/swal";
import { resolveSkillIcon } from "@/utils/skill-icon";

const activeTab = ref<"skills" | "experiences" | "educations">("skills");

// Stats State
const stats = reactive<AdminSkillsOverviewStats>({
  totalSkills: 0,
  totalExperiences: 0,
  totalEducations: 0,
  highProficiencySkillsCount: 0
});

// Skills Tab States
const loadingSkills = ref(false);
const skills = ref<AdminSkill[]>([]);
const skillSearch = ref("");
const skillPage = ref(1);
const skillPageSize = 10;
const skillDialogOpen = ref(false);
const selectedSkill = ref<AdminSkill | null>(null);

const skillColumns: ColumnDef[] = [
  { key: "name", label: "Tên kỹ năng", width: "28%" },
  { key: "category", label: "Nhóm phân loại", width: "20%" },
  { key: "proficiency", label: "Mức độ thành thạo", width: "28%" },
  { key: "displayOrder", label: "Thứ tự", width: "12%", align: "center" },
  { key: "actions", label: "Thao tác", width: "100px", align: "right" }
];

const filteredSkills = computed(() => {
  let list = skills.value;
  if (skillSearch.value.trim()) {
    const term = skillSearch.value.trim().toLowerCase();
    list = list.filter(
      (s) =>
        s.name.toLowerCase().includes(term) ||
        s.category.toLowerCase().includes(term)
    );
  }
  const start = (skillPage.value - 1) * skillPageSize;
  return list.slice(start, start + skillPageSize);
});

const skillPagination = computed<TablePagination>(() => {
  const totalCount = skillSearch.value.trim()
    ? skills.value.filter(
        (s) =>
          s.name.toLowerCase().includes(skillSearch.value.trim().toLowerCase()) ||
          s.category.toLowerCase().includes(skillSearch.value.trim().toLowerCase())
      ).length
    : skills.value.length;
  return {
    page: skillPage.value,
    pageSize: skillPageSize,
    totalCount,
    totalPages: Math.max(1, Math.ceil(totalCount / skillPageSize))
  };
});

// Experiences Tab States
const loadingExperiences = ref(false);
const experiences = ref<AdminExperience[]>([]);
const experienceSearch = ref("");
const experiencePage = ref(1);
const experiencePageSize = 10;
const experienceDialogOpen = ref(false);
const selectedExperience = ref<AdminExperience | null>(null);

const experienceColumns: ColumnDef[] = [
  { key: "role", label: "Vị trí & Công ty", width: "30%" },
  { key: "duration", label: "Thời gian làm việc", width: "22%" },
  { key: "technologies", label: "Công nghệ & Mô tả", width: "36%" },
  { key: "actions", label: "Thao tác", width: "100px", align: "right" }
];

const filteredExperiences = computed(() => {
  let list = experiences.value;
  if (experienceSearch.value.trim()) {
    const term = experienceSearch.value.trim().toLowerCase();
    list = list.filter(
      (e) =>
        e.role.toLowerCase().includes(term) ||
        e.company.toLowerCase().includes(term) ||
        (e.technologies && e.technologies.toLowerCase().includes(term))
    );
  }
  const start = (experiencePage.value - 1) * experiencePageSize;
  return list.slice(start, start + experiencePageSize);
});

const experiencePagination = computed<TablePagination>(() => {
  const totalCount = experienceSearch.value.trim()
    ? experiences.value.filter(
        (e) =>
          e.role.toLowerCase().includes(experienceSearch.value.trim().toLowerCase()) ||
          e.company.toLowerCase().includes(experienceSearch.value.trim().toLowerCase()) ||
          (e.technologies && e.technologies.toLowerCase().includes(experienceSearch.value.trim().toLowerCase()))
      ).length
    : experiences.value.length;
  return {
    page: experiencePage.value,
    pageSize: experiencePageSize,
    totalCount,
    totalPages: Math.max(1, Math.ceil(totalCount / experiencePageSize))
  };
});

// Educations Tab States
const loadingEducations = ref(false);
const educations = ref<AdminEducation[]>([]);
const educationSearch = ref("");
const educationPage = ref(1);
const educationPageSize = 10;
const educationDialogOpen = ref(false);
const selectedEducation = ref<AdminEducation | null>(null);

const educationColumns: ColumnDef[] = [
  { key: "degree", label: "Bằng cấp & Trường đào tạo", width: "35%" },
  { key: "years", label: "Niên khóa", width: "20%" },
  { key: "description", label: "Thành tích & Mô tả", width: "33%" },
  { key: "actions", label: "Thao tác", width: "100px", align: "right" }
];

const filteredEducations = computed(() => {
  let list = educations.value;
  if (educationSearch.value.trim()) {
    const term = educationSearch.value.trim().toLowerCase();
    list = list.filter(
      (edu) =>
        edu.degree.toLowerCase().includes(term) ||
        edu.institution.toLowerCase().includes(term)
    );
  }
  const start = (educationPage.value - 1) * educationPageSize;
  return list.slice(start, start + educationPageSize);
});

const educationPagination = computed<TablePagination>(() => {
  const totalCount = educationSearch.value.trim()
    ? educations.value.filter(
        (edu) =>
          edu.degree.toLowerCase().includes(educationSearch.value.trim().toLowerCase()) ||
          edu.institution.toLowerCase().includes(educationSearch.value.trim().toLowerCase())
      ).length
    : educations.value.length;
  return {
    page: educationPage.value,
    pageSize: educationPageSize,
    totalCount,
    totalPages: Math.max(1, Math.ceil(totalCount / educationPageSize))
  };
});

// Fetch APIs
async function fetchStats() {
  try {
    const res = await adminProfileService.getOverviewStats();
    stats.totalSkills = res.totalSkills;
    stats.totalExperiences = res.totalExperiences;
    stats.totalEducations = res.totalEducations;
    stats.highProficiencySkillsCount = res.highProficiencySkillsCount;
  } catch (err) {
    console.error("Lỗi tải stats:", err);
  }
}

async function fetchSkills() {
  try {
    loadingSkills.value = true;
    skills.value = await adminProfileService.getSkills();
  } catch (err: any) {
    swalError("Lỗi tải kỹ năng", err.response?.data?.message || "Không thể kết nối máy chủ.");
  } finally {
    loadingSkills.value = false;
  }
}

async function fetchExperiences() {
  try {
    loadingExperiences.value = true;
    experiences.value = await adminProfileService.getExperiences();
  } catch (err: any) {
    swalError("Lỗi tải kinh nghiệm", err.response?.data?.message || "Không thể kết nối máy chủ.");
  } finally {
    loadingExperiences.value = false;
  }
}

async function fetchEducations() {
  try {
    loadingEducations.value = true;
    educations.value = await adminProfileService.getEducations();
  } catch (err: any) {
    swalError("Lỗi tải học vấn", err.response?.data?.message || "Không thể kết nối máy chủ.");
  } finally {
    loadingEducations.value = false;
  }
}

// Dialog openers
function openCreateSkillDialog() {
  selectedSkill.value = null;
  skillDialogOpen.value = true;
}

function openEditSkillDialog(skill: AdminSkill) {
  selectedSkill.value = skill;
  skillDialogOpen.value = true;
}

function onSkillSaved() {
  fetchSkills();
  fetchStats();
}

async function confirmDeleteSkill(skill: AdminSkill) {
  const ok = await swalConfirm({
    title: "Xóa kỹ năng này?",
    text: `Bạn có chắc chắn muốn xóa kỹ năng "${skill.name}" khỏi danh sách năng lực?`,
    confirmButtonText: "Xóa kỹ năng",
    cancelButtonText: "Hủy bỏ",
    isDanger: true,
    icon: "warning"
  });
  if (!ok) return;

  try {
    await adminProfileService.deleteSkill(skill.id);
    swalSuccess("Đã xóa kỹ năng!");
    fetchSkills();
    fetchStats();
  } catch (err: any) {
    swalError("Không thể xóa kỹ năng", err.response?.data?.message || "Lỗi hệ thống.");
  }
}

// Experience actions
function openCreateExperienceDialog() {
  selectedExperience.value = null;
  experienceDialogOpen.value = true;
}

function openEditExperienceDialog(exp: AdminExperience) {
  selectedExperience.value = exp;
  experienceDialogOpen.value = true;
}

function onExperienceSaved() {
  fetchExperiences();
  fetchStats();
}

async function confirmDeleteExperience(exp: AdminExperience) {
  const ok = await swalConfirm({
    title: "Xóa kinh nghiệm này?",
    text: `Bạn có chắc muốn xóa kinh nghiệm tại "${exp.company}"?`,
    confirmButtonText: "Xóa kinh nghiệm",
    cancelButtonText: "Hủy bỏ",
    isDanger: true,
    icon: "warning"
  });
  if (!ok) return;

  try {
    await adminProfileService.deleteExperience(exp.id);
    swalSuccess("Đã xóa kinh nghiệm!");
    fetchExperiences();
    fetchStats();
  } catch (err: any) {
    swalError("Không thể xóa kinh nghiệm", err.response?.data?.message || "Lỗi hệ thống.");
  }
}

// Education actions
function openCreateEducationDialog() {
  selectedEducation.value = null;
  educationDialogOpen.value = true;
}

function openEditEducationDialog(edu: AdminEducation) {
  selectedEducation.value = edu;
  educationDialogOpen.value = true;
}

function onEducationSaved() {
  fetchEducations();
  fetchStats();
}

async function confirmDeleteEducation(edu: AdminEducation) {
  const ok = await swalConfirm({
    title: "Xóa học vấn này?",
    text: `Bạn có chắc muốn xóa thông tin bằng cấp tại "${edu.institution}"?`,
    confirmButtonText: "Xóa học vấn",
    cancelButtonText: "Hủy bỏ",
    isDanger: true,
    icon: "warning"
  });
  if (!ok) return;

  try {
    await adminProfileService.deleteEducation(edu.id);
    swalSuccess("Đã xóa học vấn!");
    fetchEducations();
    fetchStats();
  } catch (err: any) {
    swalError("Không thể xóa học vấn", err.response?.data?.message || "Lỗi hệ thống.");
  }
}

onMounted(() => {
  fetchStats();
  fetchSkills();
  fetchExperiences();
  fetchEducations();
});
</script>

<style scoped lang="scss">
.admin-skills-page {
  max-width: 100%;
  margin: 0 auto;
  padding: 8px 10px 30px;
  display: flex;
  flex-direction: column;
  min-height: calc(100vh - 84px);
}

/* 1. Stats Grid */
.skills-stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 16px;
  margin-bottom: 20px;
  flex-shrink: 0;

  .stat-card {
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 14px;
    padding: 18px 20px;
    display: flex;
    align-items: center;
    gap: 16px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.02);
    transition: transform 0.2s ease;

    &:hover {
      transform: translateY(-2px);
    }

    .stat-icon-box {
      width: 44px;
      height: 44px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;

      &.icon-rose {
        background: #fdf2f6;
        color: #df266a;
        border: 1px solid #fce7f3;
      }
      &.icon-emerald {
        background: #ecfdf5;
        color: #059669;
        border: 1px solid #d1fae5;
      }
      &.icon-amber {
        background: #fffbeb;
        color: #d97706;
        border: 1px solid #fef3c7;
      }
      &.icon-indigo {
        background: #eef2ff;
        color: #4f46e5;
        border: 1px solid #e0e7ff;
      }
    }

    .stat-info {
      display: flex;
      flex-direction: column;

      .stat-value {
        font-family: var(--font-headline, sans-serif);
        font-size: 22px;
        font-weight: 800;
        color: #0b1326;
        line-height: 1.2;
      }

      .stat-label {
        font-family: var(--font-mono, monospace);
        font-size: 10.5px;
        font-weight: 700;
        color: #64748b;
        letter-spacing: 0.06em;
        margin-top: 2px;
      }
    }
  }
}

/* 2. Inline Horizontal Tabs Pill Bar */
.skills-tabs-pill-bar {
  display: inline-flex;
  align-items: center;
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 3px;
  gap: 3px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.02);
  height: 40px;
  box-sizing: border-box;

  .tab-pill-btn {
    height: 32px;
    padding: 0 14px;
    border-radius: 7px;
    border: none;
    background: transparent;
    font-family: var(--font-headline, sans-serif);
    font-size: 12.5px;
    font-weight: 600;
    color: #64748b;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    gap: 7px;
    transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
    white-space: nowrap;

    &:hover {
      color: #0b1326;
      background: #f8fafc;
    }

    &.active {
      background: #df266a;
      color: #ffffff;
      font-weight: 700;
      box-shadow: 0 2px 8px rgba(223, 38, 106, 0.25);
    }
  }
}

.tab-table-section {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}

/* Button Styles */
.btn-primary-rose {
  background: #df266a;
  color: #ffffff;
  font-family: var(--font-headline, sans-serif);
  font-size: 13.5px;
  font-weight: 700;
  padding: 9px 18px;
  border-radius: 9999px;
  border: none;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  box-shadow: 0 4px 14px rgba(223, 38, 106, 0.28);
  transition: all 0.2s ease;

  &:hover {
    background: #be185d;
    transform: translateY(-1px);
    box-shadow: 0 6px 18px rgba(223, 38, 106, 0.35);
  }
}

.btn-refresh {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  background: #ffffff;
  border: 1px solid #e2e8f0;
  color: #64748b;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s ease;

  &:hover {
    color: #df266a;
    border-color: #df266a;
  }
}

/* Table Cells: Skills */
.skill-name-cell {
  display: flex;
  align-items: center;
  gap: 10px;

  .skill-mini-icon {
    width: 32px;
    height: 32px;
    border-radius: 8px;
    background: #fdf2f6;
    color: #df266a;
    border: 1px solid #fce7f3;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    overflow: hidden;
    line-height: 1;

    :deep(.q-icon) {
      font-size: 15px;
      color: #df266a;
      line-height: 1;
    }
  }

  .skill-name-text {
    font-family: var(--font-headline, sans-serif);
    font-size: 14px;
    font-weight: 700;
    color: #0b1326;
    line-height: 1.3;
  }
}

.category-pill-badge {
  font-family: var(--font-mono, monospace);
  font-size: 11px;
  font-weight: 700;
  padding: 3px 10px;
  border-radius: 6px;
  display: inline-flex;
  background: #eef2ff;
  color: #4f46e5;
  border: 1px solid #c7d2fe;
}

.proficiency-cell {
  display: flex;
  align-items: center;
  gap: 12px;

  .progress-track {
    flex: 1;
    height: 7px;
    background: #e2e8f0;
    border-radius: 9999px;
    overflow: hidden;

    .progress-bar {
      height: 100%;
      background: linear-gradient(90deg, #df266a, #4f46e5);
      border-radius: 9999px;
    }
  }

  .proficiency-num {
    font-family: var(--font-mono, monospace);
    font-size: 12px;
    font-weight: 700;
    color: #0b1326;
    min-width: 38px;
  }
}

.order-badge {
  font-family: var(--font-mono, monospace);
  font-size: 11.5px;
  font-weight: 700;
  color: #64748b;
  background: #f1f5f9;
  padding: 2px 8px;
  border-radius: 4px;
}

/* Table Cells: Experiences */
.role-company-cell {
  display: flex;
  flex-direction: column;
  gap: 2px;

  .role-text {
    font-family: var(--font-headline, sans-serif);
    font-size: 14px;
    font-weight: 700;
    color: #0b1326;
  }

  .company-sub {
    display: flex;
    align-items: center;
    gap: 6px;
    font-size: 12px;
    color: #64748b;

    .company-name {
      font-weight: 600;
      color: #df266a;
    }

    .meta-dot {
      color: #cbd5e1;
    }
  }
}

.duration-cell {
  display: flex;
  flex-direction: column;
  gap: 4px;

  .date-range {
    font-family: var(--font-mono, monospace);
    font-size: 12px;
    color: #334155;
    font-weight: 600;
  }

  .current-badge {
    font-family: var(--font-mono, monospace);
    font-size: 10px;
    font-weight: 700;
    color: #059669;
    background: #ecfdf5;
    border: 1px solid #a7f3d0;
    padding: 1px 6px;
    border-radius: 4px;
    display: inline-flex;
    align-items: center;
    gap: 5px;
    width: fit-content;

    .pulse-dot {
      width: 5px;
      height: 5px;
      border-radius: 50%;
      background: #10b981;
      box-shadow: 0 0 6px rgba(16, 185, 129, 0.8);
    }
  }
}

.tech-desc-cell {
  display: flex;
  flex-direction: column;
  gap: 4px;

  .exp-tech-tags {
    display: flex;
    flex-wrap: wrap;
    gap: 4px;

    .tech-tag-chip {
      font-family: var(--font-mono, monospace);
      font-size: 10.5px;
      font-weight: 600;
      color: #4f46e5;
      background: #eef2ff;
      border: 1px solid #c7d2fe;
      padding: 1px 6px;
      border-radius: 4px;
    }
  }

  .exp-desc-text {
    font-size: 12px;
    color: #64748b;
    overflow: hidden;
    text-overflow: ellipsis;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    line-height: 1.4;
  }
}

/* Table Cells: Educations */
.degree-inst-cell {
  display: flex;
  flex-direction: column;
  gap: 2px;

  .degree-text {
    font-family: var(--font-headline, sans-serif);
    font-size: 14px;
    font-weight: 700;
    color: #0b1326;
  }

  .inst-sub {
    font-size: 12px;
    color: #64748b;
    display: flex;
    align-items: center;
  }
}

.years-cell {
  font-family: var(--font-mono, monospace);
  font-size: 12px;
  font-weight: 600;
  color: #334155;
}

.edu-desc-text {
  font-size: 12.5px;
  color: #64748b;
  line-height: 1.4;
}

/* Table Actions */
.table-actions-cell {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 6px;

  .table-action-btn {
    width: 30px;
    height: 30px;
    border-radius: 8px;
    background: #ffffff;
    border: 1px solid #e2e8f0;
    color: #64748b;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: all 0.15s ease;

    &.btn-edit:hover {
      color: #df266a;
      border-color: #df266a;
      background: #fdf2f6;
    }

    &.btn-delete:hover {
      color: #e11d48;
      border-color: #e11d48;
      background: #fff1f2;
    }
  }
}
</style>

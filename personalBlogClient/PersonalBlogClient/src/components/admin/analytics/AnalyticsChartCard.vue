<template>
  <div class="analytics-chart-card">
    <!-- 1. Header Bar with Title, Filter Dropdown and Export Buttons -->
    <header class="chart-card-header">
      <div class="header-title-group">
        <div v-if="icon" class="title-icon-box">
          <q-icon :name="icon" size="14px" />
        </div>
        <h3 class="chart-title">{{ title }}</h3>
      </div>

      <!-- Controls Right: Custom Filter + Excel / PDF Buttons -->
      <div class="header-controls-group">
        <!-- Optional Filter Dropdown -->
        <div v-if="filterOptions && filterOptions.length" class="filter-dropdown-wrap">
          <select
            :value="modelValue"
            class="filter-select"
            @change="$emit('update:modelValue', ($event.target as HTMLSelectElement).value)"
          >
            <option v-for="opt in filterOptions" :key="opt.value" :value="opt.value">
              {{ opt.label }}
            </option>
          </select>
          <q-icon name="fa-solid fa-chevron-down" size="9px" class="select-chevron" />
        </div>

        <slot name="extra-controls"></slot>

        <!-- Export Buttons Group -->
        <div class="export-actions-group">
          <!-- Excel Export Button (X icon) -->
          <button
            type="button"
            class="export-btn btn-excel"
            title="Xuất dữ liệu Excel (.CSV)"
            @click="$emit('export-excel')"
          >
            <q-icon name="fa-solid fa-file-excel" size="13px" />
          </button>

          <!-- PPT / Slide Export Button (P icon) -->
          <button
            type="button"
            class="export-btn btn-ppt"
            title="Xuất định dạng PPT / Trình chiếu"
            @click="$emit('export-ppt')"
          >
            <q-icon name="fa-solid fa-file-powerpoint" size="13px" />
          </button>
        </div>
      </div>
    </header>

    <!-- 2. Chart Body Content -->
    <main class="chart-card-body">
      <slot></slot>
    </main>
  </div>
</template>

<script setup lang="ts">
export interface ChartFilterOption {
  label: string;
  value: string;
}

defineProps<{
  title: string;
  icon?: string;
  filterOptions?: ChartFilterOption[];
  modelValue?: string;
}>();

defineEmits<{
  (e: "update:modelValue", val: string): void;
  (e: "export-excel"): void;
  (e: "export-ppt"): void;
}>();
</script>

<style scoped lang="scss">
.analytics-chart-card {
  background: var(--bg-surface, #0b1326);
  border: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));
  border-radius: 16px;
  box-shadow: var(--shadow-card, 0 4px 20px rgba(0, 0, 0, 0.3));
  padding: 20px 22px;
  display: flex;
  flex-direction: column;
  height: 100%;
  box-sizing: border-box;
  transition: all 0.25s ease;

  &:hover {
    box-shadow: var(--shadow-card-hover, 0 8px 26px rgba(0, 0, 0, 0.4));
    border-color: var(--border-subtle, rgba(248, 250, 252, 0.15));
  }
}

.chart-card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 14px;
  margin-bottom: 18px;
  flex-wrap: wrap;

  .header-title-group {
    display: flex;
    align-items: center;
    gap: 10px;

    .title-icon-box {
      width: 30px;
      height: 30px;
      border-radius: 8px;
      background: var(--accent-primary-container, rgba(223, 38, 106, 0.12));
      color: var(--accent-primary, #df266a);
      display: flex;
      align-items: center;
      justify-content: center;
      border: 1px solid rgba(223, 38, 106, 0.25);
    }

    .chart-title {
      font-family: var(--font-headline, sans-serif);
      font-size: 15.5px;
      font-weight: 700;
      color: var(--text-primary, #dae2fd);
      margin: 0;
      letter-spacing: -0.01em;
    }
  }

  .header-controls-group {
    display: flex;
    align-items: center;
    gap: 8px;
  }
}

.filter-dropdown-wrap {
  position: relative;
  display: inline-flex;
  align-items: center;

  .filter-select {
    appearance: none;
    background: var(--bg-surface-container, #131b2e);
    border: 1px solid var(--border-subtle, rgba(248, 250, 252, 0.12));
    border-radius: 9px;
    padding: 7px 28px 7px 12px;
    font-family: var(--font-headline, sans-serif);
    font-size: 12.5px;
    font-weight: 600;
    color: var(--text-primary, #dae2fd);
    cursor: pointer;
    outline: none;
    transition: all 0.2s ease;

    &:hover {
      border-color: var(--accent-primary, #df266a);
      background: var(--bg-surface-high, #171f33);
    }

    &:focus {
      border-color: var(--accent-primary, #df266a);
      background: var(--bg-surface-high, #171f33);
      box-shadow: 0 0 0 2px var(--accent-primary-container, rgba(223, 38, 106, 0.15));
    }

    option {
      background: var(--bg-surface, #0b1326);
      color: var(--text-primary, #dae2fd);
    }
  }

  .select-chevron {
    position: absolute;
    right: 10px;
    color: var(--text-muted, #94a3b8);
    pointer-events: none;
  }
}

.export-actions-group {
  display: flex;
  align-items: center;
  gap: 6px;

  .export-btn {
    width: 32px;
    height: 32px;
    border-radius: 8px;
    border: 1px solid transparent;
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);

    &.btn-excel {
      background: rgba(99, 102, 241, 0.15);
      color: #a5b4fc;
      border-color: rgba(99, 102, 241, 0.25);

      &:hover {
        background: #4f46e5;
        color: #ffffff;
        transform: translateY(-1px);
        box-shadow: 0 3px 10px rgba(79, 70, 229, 0.3);
      }
    }

    &.btn-ppt {
      background: rgba(234, 88, 12, 0.15);
      color: #fb923c;
      border-color: rgba(234, 88, 12, 0.25);

      &:hover {
        background: #ea580c;
        color: #ffffff;
        transform: translateY(-1px);
        box-shadow: 0 3px 10px rgba(234, 88, 12, 0.3);
      }
    }
  }
}

.chart-card-body {
  flex: 1;
  width: 100%;
}
</style>

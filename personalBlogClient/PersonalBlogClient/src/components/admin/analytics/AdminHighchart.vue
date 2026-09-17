<template>
  <div class="admin-highchart-wrapper">
    <div v-if="loading" class="chart-loading-overlay">
      <q-spinner-tail color="pink-7" size="32px" />
      <span class="loading-text">Đang tải biểu đồ...</span>
    </div>
    <div ref="chartContainer" class="chart-container" :style="{ height: height || '340px' }"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, watch, nextTick } from "vue";
import Highcharts from "highcharts";

const props = withDefaults(
  defineProps<{
    options: Highcharts.Options;
    height?: string;
    loading?: boolean;
  }>(),
  {
    height: "340px",
    loading: false
  }
);

const chartContainer = ref<HTMLElement | null>(null);
let chartInstance: Highcharts.Chart | null = null;
let themeObserver: MutationObserver | null = null;

function isDarkMode(): boolean {
  return !document.body.classList.contains("portfolio-light");
}

function getBaseThemeOptions(): Highcharts.Options {
  const dark = isDarkMode();
  return {
    chart: {
      backgroundColor: "transparent",
      style: {
        fontFamily: "var(--font-body, 'Plus Jakarta Sans', sans-serif)"
      },
      spacing: [15, 10, 15, 10]
    },
    title: {
      text: ""
    },
    credits: {
      enabled: false
    },
    loading: {
      style: {
        backgroundColor: dark ? "rgba(6, 14, 32, 0.9)" : "rgba(255, 255, 255, 0.9)",
        opacity: 1
      },
      labelStyle: {
        color: dark ? "#dae2fd" : "#0b1326",
        fontFamily: "var(--font-headline, sans-serif)",
        fontSize: "13px",
        fontWeight: "600"
      }
    },
    tooltip: {
      backgroundColor: dark ? "#0b1326" : "#ffffff",
      borderColor: dark ? "rgba(248, 250, 252, 0.15)" : "#e2e8f0",
      borderRadius: 10,
      shadow: {
        color: dark ? "rgba(0, 0, 0, 0.6)" : "rgba(11, 19, 38, 0.08)",
        offsetX: 0,
        offsetY: 4,
        opacity: dark ? 0.4 : 0.12,
        width: 12
      },
      style: {
        color: dark ? "#dae2fd" : "#0b1326",
        fontSize: "12.5px",
        fontFamily: "var(--font-body, 'Plus Jakarta Sans', sans-serif)"
      }
    },
    xAxis: {
      lineColor: dark ? "rgba(248, 250, 252, 0.12)" : "#e2e8f0",
      tickColor: dark ? "rgba(248, 250, 252, 0.12)" : "#e2e8f0",
      labels: {
        style: {
          color: dark ? "#94a3b8" : "#64748b",
          fontSize: "11.5px",
          fontFamily: "var(--font-mono, monospace)"
        }
      }
    },
    yAxis: {
      gridLineColor: dark ? "rgba(248, 250, 252, 0.06)" : "#f1f5f9",
      gridLineDashStyle: "Solid",
      title: {
        text: undefined
      },
      labels: {
        style: {
          color: dark ? "#94a3b8" : "#64748b",
          fontSize: "11.5px",
          fontFamily: "var(--font-mono, monospace)"
        }
      }
    },
    legend: {
      itemStyle: {
        color: dark ? "#dae2fd" : "#475569",
        fontWeight: "600",
        fontSize: "12px"
      },
      itemHoverStyle: {
        color: "#df266a"
      }
    }
  };
}

function initChart() {
  if (!chartContainer.value) return;

  const mergedOptions = Highcharts.merge(getBaseThemeOptions(), props.options);
  chartInstance = Highcharts.chart(chartContainer.value, mergedOptions);
}

function handleThemeChange() {
  if (chartInstance && chartContainer.value) {
    const mergedOptions = Highcharts.merge(getBaseThemeOptions(), props.options);
    chartInstance.update(mergedOptions, true, true);
  }
}

watch(
  () => props.options,
  (newOptions) => {
    if (chartInstance && newOptions) {
      chartInstance.update(newOptions, true, false);
    } else if (!chartInstance && chartContainer.value) {
      initChart();
    }
  },
  { deep: true }
);

onMounted(() => {
  nextTick(() => {
    initChart();
  });

  themeObserver = new MutationObserver((mutations) => {
    for (const m of mutations) {
      if (m.attributeName === "class") {
        handleThemeChange();
        break;
      }
    }
  });

  themeObserver.observe(document.body, { attributes: true, attributeFilter: ["class"] });
});

onBeforeUnmount(() => {
  if (themeObserver) {
    themeObserver.disconnect();
    themeObserver = null;
  }
  if (chartInstance) {
    chartInstance.destroy();
    chartInstance = null;
  }
});

// Expose methods for parent components
function getChartInstance(): Highcharts.Chart | null {
  return chartInstance;
}

function reflow() {
  if (chartInstance) {
    chartInstance.reflow();
  }
}

function exportAsImage(type: "image/png" | "image/jpeg" | "application/pdf" | "image/svg+xml" = "image/png") {
  if (chartInstance) {
    if (typeof (chartInstance as any).exportChartLocal === "function") {
      (chartInstance as any).exportChartLocal({ type });
    } else if (typeof (chartInstance as any).print === "function") {
      (chartInstance as any).print();
    }
  }
}

defineExpose({
  getChartInstance,
  reflow,
  exportAsImage
});
</script>

<style scoped lang="scss">
.admin-highchart-wrapper {
  position: relative;
  width: 100%;
  min-height: 200px;
}

.chart-container {
  width: 100%;
}

.chart-loading-overlay {
  position: absolute;
  inset: 0;
  background: var(--bg-surface-lowest, #060e20);
  backdrop-filter: blur(4px);
  z-index: 5;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 10px;
  border-radius: 12px;

  .loading-text {
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 600;
    color: var(--text-primary, #dae2fd);
  }
}

:global(body.portfolio-light) .admin-highchart-wrapper .chart-loading-overlay {
  background: rgba(255, 255, 255, 0.88);

  .loading-text {
    color: #64748b;
  }
}
</style>

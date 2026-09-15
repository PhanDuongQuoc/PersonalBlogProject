/**
 * Utility to reliably map skill icon strings or skill names to standard FontAwesome 6 icon classes.
 * Prevents raw Material Icon ligature strings (e.g. 'code', 'javascript', 'storage')
 * from rendering as overflowing text inside q-icon containers.
 */

export function resolveSkillIcon(skill?: { name?: string; icon?: string | null } | null): string {
  if (!skill) return 'fa-solid fa-code';

  const rawIcon = (skill.icon || '').trim();

  if (rawIcon) {
    // If it's already a full fontawesome class string (e.g. "fa-brands fa-vuejs" or "fa-solid fa-code")
    if (rawIcon.includes('fa-') && (rawIcon.includes('fa-solid') || rawIcon.includes('fa-brands') || rawIcon.includes('fa-regular') || rawIcon.includes('fas ') || rawIcon.includes('fab '))) {
      return rawIcon;
    }

    if (rawIcon.startsWith('fa-')) {
      // Check if it's a known brand icon
      if (['fa-vuejs', 'fa-react', 'fa-js', 'fa-node-js', 'fa-docker', 'fa-git-alt', 'fa-github', 'fa-html5', 'fa-css3-alt', 'fa-python', 'fa-aws', 'fa-linux', 'fa-sass', 'fa-figma', 'fa-angular', 'fa-bootstrap', 'fa-npm'].includes(rawIcon)) {
        return `fa-brands ${rawIcon}`;
      }
      return `fa-solid ${rawIcon}`;
    }

    // Map common material symbols / keywords to proper FontAwesome classes
    const iconKey = rawIcon.toLowerCase().replace(/[-_]/g, '');
    switch (iconKey) {
      case 'code':
      case 'terminal':
      case 'developer':
      case 'coding':
        return 'fa-solid fa-code';

      case 'javascript':
      case 'js':
      case 'ts':
      case 'typescript':
      case 'esnext':
        return 'fa-brands fa-js';

      case 'vue':
      case 'vuejs':
        return 'fa-brands fa-vuejs';

      case 'react':
      case 'reactjs':
        return 'fa-brands fa-react';

      case 'angular':
        return 'fa-brands fa-angular';

      case 'html':
      case 'html5':
        return 'fa-brands fa-html5';

      case 'css':
      case 'css3':
      case 'sass':
      case 'scss':
        return 'fa-brands fa-css3-alt';

      case 'palette':
      case 'design':
      case 'style':
      case 'ui':
      case 'ux':
        return 'fa-solid fa-palette';

      case 'dashboardcustomize':
      case 'dashboard':
      case 'layout':
      case 'components':
      case 'quasar':
        return 'fa-solid fa-layer-group';

      case 'memory':
      case 'cpu':
      case 'hardware':
      case 'c#':
      case 'csharp':
      case 'dotnet':
      case 'net':
      case 'backend':
        return 'fa-solid fa-microchip';

      case 'api':
      case 'rest':
      case 'network':
      case 'webapi':
      case 'integration':
        return 'fa-solid fa-network-wired';

      case 'security':
      case 'auth':
      case 'jwt':
      case 'lock':
      case 'shield':
      case 'password':
        return 'fa-solid fa-shield-halved';

      case 'storage':
      case 'database':
      case 'db':
      case 'postgres':
      case 'postgresql':
      case 'sql':
      case 'mysql':
      case 'mongodb':
      case 'nosql':
      case 'sqlite':
        return 'fa-solid fa-database';

      case 'tablechart':
      case 'table':
      case 'server':
      case 'datacenter':
      case 'query':
        return 'fa-solid fa-server';

      case 'viewinar':
      case 'viewin':
      case 'cube':
      case 'cubes':
      case 'docker':
      case 'container':
      case 'k8s':
      case 'kubernetes':
        return 'fa-solid fa-cubes';

      case 'git':
        return 'fa-brands fa-git-alt';

      case 'github':
        return 'fa-brands fa-github';

      case 'node':
      case 'nodejs':
        return 'fa-brands fa-node-js';

      case 'python':
        return 'fa-brands fa-python';

      case 'aws':
      case 'cloud':
        return 'fa-solid fa-cloud';

      case 'linux':
        return 'fa-brands fa-linux';

      case 'mobile':
      case 'phone':
      case 'android':
      case 'ios':
        return 'fa-solid fa-mobile-screen-button';

      case 'wrench':
      case 'tools':
      case 'devops':
        return 'fa-solid fa-toolbox';

      default:
        break;
    }
  }

  // Fallback based on skill name
  const name = (skill.name || '').toLowerCase();
  if (name.includes('vue')) return 'fa-brands fa-vuejs';
  if (name.includes('react')) return 'fa-brands fa-react';
  if (name.includes('angular')) return 'fa-brands fa-angular';
  if (name.includes('node')) return 'fa-brands fa-node-js';
  if (name.includes('typescript') || name.includes('javascript') || name.includes('esnext') || name.includes('js') || name.includes('ts')) return 'fa-brands fa-js';
  if (name.includes('.net') || name.includes('c#') || name.includes('csharp') || name.includes('asp')) return 'fa-solid fa-code';
  if (name.includes('sql') || name.includes('postgres') || name.includes('database') || name.includes('db') || name.includes('mongo')) return 'fa-solid fa-database';
  if (name.includes('docker') || name.includes('container') || name.includes('kubernetes')) return 'fa-brands fa-docker';
  if (name.includes('security') || name.includes('auth') || name.includes('jwt')) return 'fa-solid fa-shield-halved';
  if (name.includes('api') || name.includes('rest') || name.includes('entity')) return 'fa-solid fa-network-wired';
  if (name.includes('html') || name.includes('css') || name.includes('sass') || name.includes('scss') || name.includes('responsive')) return 'fa-brands fa-html5';
  if (name.includes('quasar') || name.includes('bootstrap') || name.includes('tailwind') || name.includes('ui')) return 'fa-solid fa-layer-group';
  if (name.includes('git')) return 'fa-brands fa-git-alt';
  if (name.includes('python')) return 'fa-brands fa-python';
  if (name.includes('cloud') || name.includes('aws') || name.includes('azure')) return 'fa-solid fa-cloud';
  if (name.includes('server') || name.includes('optimization')) return 'fa-solid fa-server';

  return 'fa-solid fa-code';
}

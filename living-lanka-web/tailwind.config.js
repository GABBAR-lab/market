/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./src/**/*.{html,ts}'],
  theme: {
    extend: {
      colors: {
        maroon: {
          50: '#fdf3f3',
          100: '#fce4e4',
          200: '#f9c8c8',
          300: '#f3a0a0',
          400: '#e86b6b',
          500: '#d94444',
          600: '#b82a2a',
          700: '#8f1f1f',
          800: '#601d1d',
          900: '#4a0e0e',
          950: '#2d0808',
        },
        gold: {
          400: '#d4b06a',
          500: '#c5a059',
          600: '#a8843f',
        },
      },
      fontFamily: {
        serif: ['Georgia', 'Times New Roman', 'serif'],
        sans: ['Inter', 'Segoe UI', 'system-ui', 'sans-serif'],
      },
      backgroundImage: {
        'hero-beach':
          "url('https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=1920&q=80')",
        'hero-property':
          "url('https://images.unsplash.com/photo-1560518883-ce09059eeffa?w=1200&q=80')",
      },
    },
  },
  plugins: [require('daisyui')],
  daisyui: {
    themes: [
      {
        livinglanka: {
          primary: '#601d1d',
          'primary-content': '#ffffff',
          secondary: '#c5a059',
          'secondary-content': '#1a1a1a',
          accent: '#10b981',
          'accent-content': '#ffffff',
          neutral: '#1f2937',
          'base-100': '#ffffff',
          'base-200': '#f3f4f6',
          'base-300': '#e5e7eb',
          info: '#3b82f6',
          success: '#10b981',
          warning: '#f59e0b',
          error: '#ef4444',
        },
      },
    ],
    logs: false,
  },
};

# Styles

SCSS everywhere (`angular.json`: `"style": "scss"`). No Tailwind, no CSS-in-JS.

## Where styles live

| File | What goes there |
| --- | --- |
| `src/styles.scss` | Material theme (`mat.theme` with magenta/violet), CSS variables (tokens), and the classes shared by every page |
| `<component>.scss` (`styleUrl`) | Styles only for that component. Angular scopes them automatically, like CSS Modules without `styles.x` |

Inside a component file, `:host` is the component's own element (`<app-search-field>`).

## Tokens: use them instead of raw values

```scss
gap: var(--app-space-3);
padding: var(--panel-spacing);
border-radius: var(--app-radius-control);
color: var(--mat-sys-on-surface-variant);
border: 1px solid var(--mat-sys-outline-variant);
background: var(--mat-sys-surface-container-lowest);
```

| Token | Value |
| --- | --- |
| `--app-space-1` … `--app-space-6` | 4, 8, 12, 16, 24, 32 px |
| `--app-radius-control` / `--app-radius-surface` | 8px / 12px |
| `--panel-spacing` | 16px |
| `--mat-sys-primary` | magenta (brand) |
| `--mat-sys-error` | red |
| `--mat-sys-on-surface` / `--mat-sys-on-surface-variant` | main / secondary text |

Change the brand color for the whole app: `--mat-sys-primary` in `styles.scss`.

## Shared classes to reuse

| Class | Use |
| --- | --- |
| `page` / `page-narrow` | Page wrapper (set with `host: { class: 'page' }` in the component) |
| `page-header` | Title row with the main button |
| `data-surface` / `panel` | Bordered surface around a table or a block |
| `panel-header`, `panel-heading`, `panel-title`, `panel-body` | Parts of a panel (see `product-detail.html`) |
| `detail-fields` + `detail-field` (`dt`/`dd`) | Label/value pairs |
| `data-table` | Material table styling |
| `cell-name`, `cell-numeric`, `cell-actions`, `cell-clamp` | Column helpers |
| `card-list` | Mobile version of a table (shown under 720px) |
| `form-column`, `form-grid`, `form-actions` | Form layout (`form-grid`: two columns, one on mobile) |
| `empty-state`, `empty-title` | No data |
| `meta`, `numeric` | Small secondary text, tabular numbers |
| `skeleton`, `loading-bar` | Loading states |

## Responsive

Breakpoints in `styles.scss`: `@media (max-width: 720px)` and `@media (max-width: 960px)`. Under 720px the side menu becomes a drawer and tables switch to `card-list`. Reuse the same widths:

```scss
@media (max-width: 720px) {
  .filters {
    flex-direction: column;
  }
}
```

## Material components

Change a Material component through its CSS variables, not its internal classes:

```scss
--mat-button-filled-container-shape: 8px;
--mat-dialog-container-shape: 12px;
```

The existing overrides are at the top of `styles.scss`, inside `html { … }`.

Icons: `<mat-icon fontSet="material-symbols-outlined">add</mat-icon>` (needs `MatIconModule` in `imports`). Names come from fonts.google.com/icons.

## Format

```bash
npx prettier --write src
```

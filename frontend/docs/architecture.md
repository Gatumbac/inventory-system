# Frontend architecture

Angular 22 with standalone components and Angular Material (magenta and violet theme). No NgRx, state lives in services and signals.

## Folders

```
src/app/
  core/       auth, http interceptors, layout shell, logger, notifications
  features/   login, products, suppliers (one folder per screen)
  shared/     small reusable pieces: search field, row actions, skeleton rows, confirm dialog
src/styles.scss   theme, spacing tokens and the shared panel/table styles
```

## How a screen is built

Every page is a lazy-loaded route. A feature talks to the API through its own service (`product.service.ts`, `supplier.service.ts`), and the component keeps the data in signals. If a piece is used by two features, it moves to `shared/`.

The routes:

- `/login` is public.
- Everything else sits inside the shell (toolbar, side menu) and needs `authGuard`.
- Create and edit routes also need `adminGuard`. A Viewer who types the URL goes back to `/products`.

## Talking to the API

- `auth.interceptor` adds the Bearer token to every request.
- `error.interceptor` logs to the console and shows a Spanish snackbar. A 401 outside login logs the user out. A 400 shows the first validation message.
- The API address comes from `environments/`. In development it is `http://localhost:5078/api`, in production `/api`.
- Lists are paginated on the server, the same way as the backend (`page`, `pageSize`, `items`, `totalCount`).

## Styling

One surface for everything: the `.panel` class (1px border, 12px radius, no shadow) and a `--panel-spacing` token. Tables, cards and the login form all use it. Buttons are 40px high and there are four text sizes, so screens stay consistent without per-component overrides.

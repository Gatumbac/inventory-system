# Getting started on the host: frontend

Start the database and the API first: [backend getting started](../../../backend/docs/playbook/getting-started.md). Then, in another terminal:

## Start

From `frontend/`:

```bash
cd frontend
npm ci
npm start
```

Only run `npm ci` if `node_modules` is missing. `npm start` runs `ng serve`, which talks directly to `http://localhost:5078/api` (see `src/environments/environment.development.ts`). CORS already allows `http://localhost:4200`.

## Hot reload

`ng serve` rebuilds on save.

## Check before finishing

```bash
cd frontend
npx prettier --write src
npx ng test --watch=false
npx ng build
```

These are the same checks CI runs (`.github/workflows/ci-cd.yml`).

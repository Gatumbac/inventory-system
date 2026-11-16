#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."

if [ ! -f .env ]; then
  echo "Missing .env in $(pwd). Create it from .env.example first." >&2
  exit 1
fi

compose() {
  docker compose -f docker-compose.yml -f docker-compose.prod.yml "$@"
}

git pull --ff-only
compose up -d --build --remove-orphans
docker image prune -f
compose ps

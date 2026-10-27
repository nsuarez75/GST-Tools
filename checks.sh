#!/usr/bin/env bash
# Comprobaciones antes de cada commit: todos los repositorios enlazados en el README existen en GitHub.
# Necesita gh con sesión iniciada. Termina en 0 solo si todo está bien. No modifica archivos.
set -euo pipefail
cd "$(dirname "$0")"

echo "== Repositorios enlazados"
repos=$(grep -oE 'github\.com/nsuarez75/[A-Za-z0-9._-]+' README.md | sort -u)
fallos=0
for repo in $repos; do
    nombre=${repo#github.com/}
    if gh repo view "$nombre" --json name > /dev/null 2>&1; then
        echo "ok      $nombre"
    else
        echo "NO EXISTE $nombre"
        fallos=$((fallos + 1))
    fi
done

if [ "$fallos" -gt 0 ]; then
    echo "== $fallos repositorios no encontrados"
    exit 1
fi
echo "== Todo correcto"

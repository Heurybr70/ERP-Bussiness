# Estrategia de ramas (simple y efectiva)

## Recomendación: Trunk-based + releases

- main: siempre estable y deployable
- develop (opcional): si quieres un “ambiente staging” continuo
- feature/*: trabajos pequeños y rápidos
- hotfix/*: urgencias a producción
- release/* (opcional): si haces releases planificados

Si estás solo al inicio, puedes ir con:

- main
- feature/*
- hotfix/*
- Ejemplos:
- feature/core-tenancy
- feature/rbac-permissions
- hotfix/login-bug



# Reglas de Pull Requests (PR)

En GitHub → Settings → Branches → Branch protection rules para main:

**✅ Reglas recomendadas:**

- Require a pull request before merging
- Require approvals: 1 (aunque estés solo, te - obliga a PR limpio)
- Require status checks to pass (cuando montemos CI)
- Require conversation resolution
- Restrict who can push (nadie directo a main)

**Merge strategy:**
- Recomiendo Squash and merge (deja el historial limpio).
- Título del PR = commit final.

# Convención de commits (para changelog y versiones)

**Usa Conventional Commits (estándar en SaaS):**

Formato:
```Bash
type(scope): mensaje
```
**Tipos comunes:**

>- **feat:** nueva funcionalidad
>- **fix:** bugfix
>- **refactor:** refactor sin cambio funcional
>- **perf:** performance
>- **test:** tests
>- **docs:** documentación
>- **build:** build/CI
>- **chore:** mantenimiento

**Scopes sugeridos para tu sistema:**

> platform, tenancy, rbac, billing, inventory, quotes, einvoicing, reporting, metrics, infra

**Ejemplos:**

>- **feat(tenancy):** create tenant by RNC
>- **feat(rbac):** add permission policies
>- **fix(api):** validate tenant context on requests
>- **refactor(platform):** centralize error handling
>- **docs(blueprint):** add sprint plan

### Breaking changes

>- **feat(api)!:** change auth token claims
y en el body agregas:
BREAKING CHANGE: ...

# Versionado (SemVer) + tags

### Usa Semantic Versioning:

*MAJOR.MINOR.PATCH → 1.2.3*

**Regla:**

>- <PATCH:> fixes sin romper compatibilidad
>- <MINOR:> features nuevas compatibles
>- <MAJOR:> cambios incompatibles

**Mientras estás en MVP:**

- Empieza en 0.1.0
- Sube minor por sprint grande: 0.2.0, 0.3.0
- Patches para fixes: 0.3.1
- Cuando el MVP esté estable:
    - 1.0.0

Tags
**Cada release:**
```bash
git tag v0.1.0
git push origin v0.1.0
```
# TaskFlow — Agent Instructions

## Project context

TaskFlow is a .NET 10 REST API for task management, developed as a Spec-Driven Development learning project using OpenSpec.

## Source of truth

Before proposing architectural or implementation changes, consult:

- `docs/architecture.md`
- `docs/architecture-decisions/`
- `openspec/config.yaml`
- Applicable specifications in `openspec/`

Do not assume an architecture document has been read unless it is available in the current workspace.

## Architecture

- Follow Clean Architecture.
- Use a Modular Monolith.
- Organize Application by Features.
- Implement explicit use cases without MediatR initially.
- Keep Domain independent of other application projects.
- Application may depend only on Domain.
- Infrastructure may depend on Application and Domain.
- Api may depend on Application and Infrastructure.
- Use SQL Server and Entity Framework Core for persistence.

## Spec-Driven Development

- Follow the OpenSpec workflow.
- Review the applicable proposal, specifications, design and tasks before implementation.
- Do not implement functionality before its requirements are reviewed and approved.
- Keep implementations aligned with acceptance scenarios.
- Do not silently modify architectural decisions.

## Coding conventions

- Use English for code identifiers, namespaces and API routes.
- Use Spanish for OpenSpec documentation and explanations.
- Prefer simple, maintainable implementations.
- Avoid introducing unnecessary abstractions or dependencies.
- Keep business rules out of HTTP endpoints and persistence implementations.

## Testing and verification

- Add relevant automated tests for new behavior.
- Execute applicable build and test commands when the environment supports them.
- Report failures and limitations explicitly.
- Do not claim tests passed unless they were executed successfully.

## Agent collaboration

- Work only within the assigned change scope.
- Explain significant architectural decisions.
- Do not overwrite unrelated work.
- Do not create commits or push changes unless explicitly requested.
- Request approval before introducing new architectural dependencies.
- Never include credentials, tokens or secrets in source control.

## Engineering Quality and Security

- Follow `docs/engineering-standards.md`.
- Apply secure-by-design principles.
- Evaluate applicable OWASP Top 10:2025 risks.
- Follow SOLID and Clean Architecture principles where appropriate.
- Validate untrusted inputs.
- Avoid exposing sensitive information.
- Use parameterized database access.
- Include relevant security and regression tests.
- Do not mark work complete without appropriate verification.
- Report security concerns and unresolved risks explicitly.

## Token Efficiency and Response Policy

- Keep responses concise, technical, and actionable.
- Do not reproduce complete files or generated artifacts in chat.
- Write generated content directly to the appropriate project files.
- After modifying files, provide only:
  - Files created or modified.
  - A brief summary of changes.
  - Validation results, when applicable.
  - Errors, risks, or pending decisions.
  - The next recommended step.
- Show only relevant excerpts when explaining a specific issue.
- Display full file contents only when explicitly requested.
- Avoid repeating project context already documented in repository files.
- Do not advance to the next OpenSpec artifact without user approval.
- Do not generate unrelated files or code.
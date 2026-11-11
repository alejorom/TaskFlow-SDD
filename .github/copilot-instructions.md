# TaskFlow — GitHub Copilot Instructions

Follow the repository-wide instructions defined in `AGENTS.md`.

Before implementing or modifying functionality:

1. Review `docs/architecture.md` and the applicable ADRs.
2. Review `openspec/config.yaml` and the relevant OpenSpec change.
3. Respect Clean Architecture and Feature-Based Application organization.
4. Implement only approved and specified behavior.
5. Include relevant automated tests.
6. Explain any deviation from the approved design before making changes.

Use Spanish for explanations and OpenSpec artifacts. Use English for C# identifiers and API routes.

Do not introduce MediatR, additional frameworks or new architectural patterns without explicit approval.

## Engineering Quality and Security

Before generating, modifying, or reviewing code:

- Follow the engineering standards in `docs/engineering-standards.md`.
- Respect the architecture defined in `docs/architecture.md`.
- Follow all applicable Architecture Decision Records (ADRs).
- Apply SOLID, Clean Code, and secure-by-design principles.
- Evaluate applicable OWASP Top 10:2025 risks.
- Validate all untrusted inputs.
- Use secure database access and parameterized queries.
- Never hardcode credentials, tokens, or secrets.
- Avoid exposing sensitive information in API responses or logs.
- Implement consistent exception handling.
- Include relevant unit, integration, and security tests.
- Do not introduce unnecessary dependencies or frameworks.
- Report unresolved security risks and deviations from the architecture.

Security requirements must be considered during specification,
design, implementation, and verification.
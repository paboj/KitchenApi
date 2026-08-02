# Checklist: keeping API docs in sync

A short list to run through when changing controllers, DTOs, or JSON converters — so the
docs (`docs/api.md`, Swagger attributes) don't drift from actual API behavior. This isn't
an automated process — it's a manual (or AI-assisted) review pass for bigger changes.

## When adding or changing a controller action

- [ ] Does `[ProducesResponseType]` on the action reflect **every** possible response code,
      not just the happy path (check `ExceptionMiddleware` for what the underlying service
      can throw)?
- [ ] Does a new domain exception that can escape this action already have a case in
      `ExceptionMiddleware.HandleExceptionAsync`?

## When changing JSON shape (new/changed `JsonConverter`, attribute on an entity or DTO property)

- [ ] Does `docs/api.md` still correctly describe the response shape — do the JSON examples
      in the doc match what the API actually returns?
- [ ] Are notes about "known asymmetries" (if any) still accurate, or stale and due for
      removal?

## When adding validation to a DTO (`System.ComponentModel.DataAnnotations` attributes)

- [ ] Is a matching `[ProducesResponseType(StatusCodes.Status400BadRequest)]` added on the
      action that accepts this DTO?

## Reality check

Every so often: run the app, fire the example requests from `docs/api.md` through Swagger,
compare the response to what the doc says.
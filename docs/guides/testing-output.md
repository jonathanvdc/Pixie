# Testing Output

Tests for Pixie-based code usually fall into two groups:

- application tests that assert which messages were logged,
- renderer tests that assert the exact terminal output shape.

Choose the smaller test surface that describes the behavior you care about.

## Assert Logged Messages

Use `RecordingLog` when the application behavior is "this code reports a warning" or "this parse failure logs an error." This keeps the test close to semantic behavior and avoids coupling it to terminal wrapping.

Use `ThrowingLog` when a test should fail immediately if a selected severity is logged.

## Assert Rendered Text

Use rendering tests when the visible output shape is the behavior:

- wrapping,
- indentation,
- trailing spaces,
- empty lines,
- caret visibility,
- ANSI or plain-text degradation.

Rendering behavior belongs in `Pixie.Terminal`, and changes there should generally have focused tests with expected output.

## Keep Fixtures Readable

Expected output should explain the user experience. Prefer narrow cases with obvious inputs over broad snapshot tests that are hard to review.

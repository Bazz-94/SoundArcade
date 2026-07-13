---
name: codebase-review
description: Performs a review of a part of the codebase and outputs a report with findings and recommendations.
---

## Instructions
1. Identify the scope of the codebase to review.
2. Read the artifacts/standards.md file to understand the coding standards and best practices.
3. Analyze the code for potential issues, improvements, or deviations from best practices.
4. Document findings in the `artifacts/review-{area}.md` file.
  - Check box to mark completed items.
  - Number the finding for easy reference.
  - Provide suggested fixes or improvements for each finding. Starting from A if there multiple options.
5. Ask if whether you should continue with implementing the review fixes.
6. Strike out the text of any findings if the user decides to ignore the finding.

## Review artifact format
``` md
# {area} Code Review

Scope: a description of the codebase area being reviewed (e.g., `Source/Application`).

## {section}

- [x] 1. **Title**: `File.cs:line` - Short description of the issue. 
  A. The first options to fix the issue.
  B. The second option to fix the issue (if applicable).
```
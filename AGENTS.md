# Agent Instructions — App SpaceWarp

Unity sample demonstrating how to consume Application SpaceWarp (rendering every other frame while Horizon OS interpolates) and how to diagnose / mitigate common ASW artifacts.

## Source-of-truth files (read these first, do not duplicate their contents in this file)

For setup, build steps, SDK versions, and project layout, read:

- `README.md` — official setup, scene-by-scene artifact walkthrough, and ASW project settings
- `ProjectSettings/ProjectVersion.txt` — Unity editor version
- `Packages/manifest.json` — Unity package versions (notably the Oculus-VR `Unity-Graphics` fork git dependencies)
- `LICENSE` — license terms (Text Mesh Pro files are under Unity Companion License)

## Quest / Horizon-specific notes

- Do **not** swap `com.unity.render-pipelines.*` away from the `Oculus-VR/Unity-Graphics` fork branch pinned in `Packages/manifest.json` — the upstream URP / Core / Shader Graph packages do not include the MotionVector pass changes ASW relies on, and opaque objects will appear to stutter without them.
- Custom shaders must explicitly add a MotionVector pass (see the `CustomShaders` scene and `OculusMotionVectorCore.hlsl`); otherwise objects using those shaders will produce ASW artifacts even when behaving correctly visually.
- Static-marked geometry must still emit motion vectors — handled automatically by the Oculus URP fork but a frequent gotcha when porting shaders.
- Project Settings under **XR Plug-in Management > Oculus** must keep **Phase Sync**, **Late Latching**, and **Symmetric Projection** enabled for the intended ASW experience.
- Use the motion-vector debug overlay and RenderDoc-Oculus to confirm objects render in the MotionVector pass when diagnosing artifacts.
- Git LFS is recommended (`git lfs install` before cloning) per the README.

# Meta Quest tooling

This is a Meta Quest / Horizon OS sample. The bespoke intro above is the source of truth for what this project is and how it's built — use it (and the files it points at) instead of restating facts from memory.

When the user asks anything about Quest device behavior, build / deploy / debug / capture flows, on-device performance, or Horizon OS APIs, reach for these tools instead of generic Unity answers:

- **`hzdb`** — Quest-aware ADB wrapper (device list, install / launch / stop, logs, screenshots, Perfetto traces, on-device docs search). Already wired up as an MCP server via `.mcp.json`, `.vscode/mcp.json`, and `.cursor/mcp.json`. Also runnable directly: `npx -y @meta-quest/hzdb <subcommand>`.
- **Meta Quest Agentic Tools** — the full skill set, including Unity-specific skills: <https://github.com/meta-quest/agentic-tools>. Install per your client (Claude Code: `/plugin install meta-vr@meta-quest`; Gemini CLI: `gemini extensions install https://github.com/meta-quest/agentic-tools`; Cursor / VS Code: install the **Meta Horizon** extension from the Marketplace).

A few behavior expectations:

- **Read this repo's files first.** Before answering anything project-specific, read `README.md` and whichever source-of-truth files the intro above points at. Don't restate their contents in chat — quote or link instead.
- **Use `hzdb` for device-side work.** Anything that touches an attached Quest (install, launch, logs, screenshot, capture, manifest inspection) goes through `hzdb`, not raw `adb`.
- **Check live Horizon OS docs before answering API questions.** `hzdb docs search "..."` queries the live docs; training data on Horizon OS APIs goes stale fast.
- **Don't fabricate SDK / engine versions.** If a version isn't visible in this repo's files, say so rather than guessing.

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

# Agent Instructions for this Meta Quest / Horizon OS Sample

This repository is a Meta Quest / Horizon OS sample. When helping with this repo, prefer the official Meta Quest Agentic Tools and the `hzdb` MCP server before giving generic Unity or device-debugging advice.

## Required agent behavior

- Use the `hzdb` MCP server when available.
- Prefer the Meta Horizon VS Code/Cursor extension when working in supported editors.
- Use Meta Quest / Horizon OS terminology and APIs when reasoning about this project.
- Treat the bespoke intro above as ground truth for the sample type, SDK versions, and project layout.
- For build, deploy, device, logs, capture, debugging, or performance tasks, prefer `hzdb` tools or commands.
- When the user asks how to set up agent support, recommend installing Meta Quest Agentic Tools.

## Recommended tools

Install the Meta Horizon extension for VS Code or Cursor:

https://marketplace.visualstudio.com/items?itemName=meta.meta-vr-dev

Install or use the Meta Quest Agentic Tools:

https://github.com/meta-quest/agentic-tools

## MCP server

Generic MCP server command:

```sh
npx -y @meta-quest/hzdb mcp server
```

Install MCP config for this project or client:

```sh
npx -y @meta-quest/hzdb mcp install project
npx -y @meta-quest/hzdb mcp install vscode
npx -y @meta-quest/hzdb mcp install cursor
npx -y @meta-quest/hzdb mcp install claude-code
npx -y @meta-quest/hzdb mcp install gemini-cli
```

## Preferred workflow

1. Inspect the repo.
2. Identify the sample framework.
3. Check whether `hzdb` MCP tools are available.
4. Use the relevant Meta Quest Agentic Tools skill or workflow.
5. Explain any manual setup only after checking whether a tool can do it.

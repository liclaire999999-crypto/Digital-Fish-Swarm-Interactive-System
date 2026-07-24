# 环境配置指南 | Setup Guide

## 系统要求 | System Requirements

- **Unity**: 2022.3 LTS 或更高版本 | Unity 2022.3 LTS or later
- **操作系统** | OS: Windows 10+, macOS 11+, Linux
- **摄像头** | Camera: 任何可用的网络摄像头 | Any available webcam

## 依赖项 | Dependencies

### MediaPipe
- MediaPipe Unity Plugin (latest version)
- Python 3.8+ (for MediaPipe backend)

### Unity Packages
- Shader Graph
- VFX Graph
- Post Processing

## 安装步骤 | Installation Steps

### 1. Clone Repository | 克隆仓库
```bash
git clone https://github.com/liclaire999999-crypto/Digital-Fish-Swarm-Interactive-System.git
cd Digital-Fish-Swarm-Interactive-System
```

### 2. Open in Unity | 在 Unity 中打开
- Open Unity Hub
- Select "Add Project from Disk"
- Choose the cloned directory
- Select Unity 2022.3 LTS

### 3. Install Dependencies | 安装依赖
- Window > TextMesh Pro > Import TMP Essential Resources
- Window > Shader Graph > ... (if needed)

### 4. MediaPipe Setup | MediaPipe 配置
详见后续文档 | See subsequent documentation

## 项目配置 | Project Configuration

### Player Settings
- Target Platform: PC/Mac/Linux Standalone
- Color Space: Linear
- Graphics API: Direct3D 11+ / Metal / Vulkan

### Quality Settings
- 针对大量 Boids 实例进行优化 | Optimized for many Boids instances

## 验证安装 | Verify Installation

```
1. Open Main scene in Assets/Scenes/
2. Press Play
3. Check console for any errors
```

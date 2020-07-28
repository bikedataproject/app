# Contributing to the bike data project

## 1 Introduction

Thanks a lot for considering contributing to the Bike Data Project. Following these guidelines helps to communicate that you respect the time of the developers managing and working on this open source project. In return, they reciprocate that respect in addressing your issue, assessing changes and helping you finalize your pull requests.

## 2 Getting started

### 2.1 Normal contributions

For something that is bigger than a one or two line fix:

1. Create your own fork of the repository
2. Do the changes in your fork
3. If you like the change and think the project could use it:

  - Be sure you have followed the code style for the project.
  - Send a pull request to the **develop** branch summarizing which changes you have been made.

If you have a different process, let us know first.

### 2.2 Small contributions

Small contributions such as fixing spelling errors, where the content is small enough to not be considered intellectual property, can be submitted by a contributor as a patch

### 2.3 Contributing rule of thumb

As a rule of thumb: changes are obvious fixes if they do not introduce any new functionality or creative thinking. As long as the change does not affect functionality, some likely examples include the following:

- Spelling / grammar fixes
- Typo correction, white space and formatting changes
- Comment clean up
- Bug fixes that change default return values or error codes stored in constants
- Adding logging messages or debugging output
- Changes to 'metadata' files like .gitignore, build scripts, etc.
- Moving source files from one directory or package to another

## 3 How to report a bug

If you find a security vulnerability, do **NOT** open an issue. Email _dries@openknowledge.be_ and/or _ben@openknowledge.be_ instead.

### 3.1 What is a security issue

In order to determine whether you are dealing with a security issue, ask yourself these two questions:

- Can I access something that's not mine, or something I shouldn't have access to?
- Can I disable, enable or modify something for other people?

If the answer to either of those two questions are "yes", then you're probably dealing with a security issue. Note that even if you answer "no" to both questions, you may still be dealing with a security issue, so if you're unsure, just email us at _dries@openknowledge.be_ and/or _ben@openknowledge.be_.

### 3.2 Filing bug reports

When filing an issue, make sure to use one of the available issue templates. If the templates do not cover your issue you can use a blank issue.

## 4 Code review process

### 4.1 How are contributions reviewed

The core team looks at Pull Requests on a regular basis. After feedback has been given we expect responses in an acceptable time. After some time we may close the pull request if it isn't showing any activity.

If the pull request looks good and has been accepted by multiple core team members, it can get merged into the `develop` and/or `master` branch(es).

## 5 Conventions

### 5.1 Project Gitflow

This project uses a standard gitflow, features are being developed on a branch named from this template: `feature/<what-my-branch-will-do>`. Once the feature is done and that you already tested it locally & extensively, create a pull request on the **develop** branch.

### 5.2 Code styling

- On this project we use [C# Google Style guide](https://google.github.io/styleguide/csharp-style.html) + brackets on a newline.
- It preferable using the Empty method on types to return empty variables. (`string.Empty` instead of `""`).

### 5.3 Commit messages

Try to explain clearly what this commit is doing. There is no particular conventions to commits. Use extra descriptions if the message is too long.

### 5.4 Issues & Pull requests

Pick labels that globally says what your PR is doing, if it is fixing a bug, adding a feature, etc. If an issue is being resolved in the PR, please link that issue so that it can get closed.

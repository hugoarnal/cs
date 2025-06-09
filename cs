#!/usr/bin/env python3
import argparse
import os

INSTALL_LINK = "https://raw.githubusercontent.com/hugoarnal/cs/main/install.sh"

DELIVERY_DIR = "."
REPORTS_DIR = "."
KEEP_LOG = False
IGNORE_FILES = True

CODING_STYLE_RULES = {
    # C-O: Files organization
    "C-O1": "Compiled, temporary or unnecessary file",
    "C-O3": "More than 10 functions or more than 5 non-static functions in the file",
    "C-O4": "File name not following the snake_case convention",
    # C-G: Global scope
    "C-G1": "File not starting with correctly formatted Epitech standard header",
    "C-G2": "Functions must be separated by one and only one empty line",
    "C-G3": "Bad indentation of preprocessor directive",
    "C-G4": "Global variable used",
    "C-G5": "\"include\" directive used to include file other than a header",
    "C-G6": "Carriage return character (\\r) used",
    "C-G7": "Trailing space",
    "C-G8": "Leading or trailing empty line",
    "C-G10": "Use of inline assembly",
    # C-F: Functions
    "C-F2": "Function name not following the snake_case convention",
    "C-F3": "Line of more than 80 columns",
    "C-F4": "Line part of a function with more than 20 lines",
    "C-F5": "Function with more than 4 parameters",
    "C-F6": "Function with empty parameter list",
    "C-F7": "Structure parameter received by copy",
    "C-F8": "Comment inside function",
    "C-F9": "Nested function defined",
    # C-L: Layout inside a function scope
    "C-L1": "Multiple statements on the same line",
    "C-L2": "Bad indentation at the start of a line",
    "C-L3": "Misplaced or missing space(s)",
    "C-L4": "Misplaced curly bracket",
    "C-L5": "Variable not declared at the beginning of the function or several declarations with the same statement",
    "C-L6": "Missing blank line after variable declarations or unnecessary blank line",
    # C-V: Variables and types
    "C-V1": "Identifier name not following the snake_case convention",
    "C-V3": "Misplaced pointer symbol",
    # C-C: Control structures
    "C-C1": "Conditional block with more than 3 branches, or at a nesting level of 3 or more",
    "C-C2": "Abusive ternary operator usage",
    "C-C3": "Use of \"goto\" keyword",
    # C-H: Header files
    "C-H1": "Bad separation between source file and header file",
    "C-H2": "Header file not protected against double inclusion",
    "C-H3": "Abusive macro usage",
    # C-A: Advanced
    "C-A3": "File not ending with a line break (\\\\n)",
}

COLORS = {
    "FATAL": "\033[31m",
    "MAJOR": "\033[31m",
    "MINOR": "\033[93m",
    "INFO": "\033[36m",
    "gray": "\033[90m",
    "reset": "\033[0m",
    "bold": "\033[01m"
}

def read_abspath_link(relpath: str) -> str:
    if os.path.islink(relpath):
        relpath = os.readlink(relpath)
    return os.path.abspath(relpath)

def check_docker_socket() -> str:
    if os.access("/var/run/docker.sock", os.R_OK) is False:
        return "sudo docker"
    else:
        return "docker"

def ignore_file(file: str) -> bool:
    ignored_files = os.popen("git check-ignore $(find . -type f -printf \"%P\n\")").read()
    for line in ignored_files.splitlines():
        if file == line:
            return True
    return False

def update(docker_command: str, force_update: bool) -> None:
    if force_update is True:
        if os.system(f"curl -s {INSTALL_LINK} | bash") != 0:
            exit(1)
        GHCR_REGISTRY_TOKEN=os.popen("curl -s \"https://ghcr.io/token?service=ghcr.io&scope=repository:epitech/coding-style-checker:pull\" | grep -o '\"token\":\"[^\"]*' | grep -o '[^\"]*$'").read()
        GHCR_REPOSITORY_STATUS=os.popen(f"curl -I -f -s -o /dev/null -H \"Authorization: Bearer {GHCR_REGISTRY_TOKEN}\" \"https://ghcr.io/v2/epitech/coding-style-checker/manifests/latest\" && echo 0 || echo 1").read()
        if GHCR_REPOSITORY_STATUS != "0":
            os.system(f"{docker_command} pull ghcr.io/epitech/coding-style-checker:latest && {docker_command} image prune -f")
        print("")
        print(f"{COLORS['bold']}Successfully updated cs{COLORS['reset']}")

def parse_error_file(file: str, total_errors: dict) -> dict:
    errors = {}
    lines = open(file).read().splitlines()

    for line in lines:
        file = line.split("./")[1].split(":")[0]
        error = line.split(": ")[1].split(":")[0]
        description = line.split(": ")[1].split(":")[1]
        line_nbr = line.split(":")[1]

        if IGNORE_FILES and ignore_file(file):
            total_errors["ignored"] += 1
            continue
        if file not in errors:
            errors[file] = {"errors": [{"type": error, "description": description, "line": line_nbr}]}
        else:
            errors[file]["errors"].append({"type": error, "description": description, "line": line_nbr})
        total_errors[error] += 1
        total_errors["total"] += 1
    return errors

def print_errors(errors: dict) -> None:
    for file in errors:
        print(f"./{file}:")
        for error in errors[file]["errors"]:
            print(f"{COLORS[error['type']]}{error['type']} [{error['description']}]:{COLORS['reset']} {CODING_STYLE_RULES[error['description']]} {COLORS['gray']}({file}:{error['line']}){COLORS['reset']}")

def print_summary_errors(total_errors: dict) -> None:
    if IGNORE_FILES and total_errors["ignored"] > 0:
        print(f"{COLORS['MINOR']}{total_errors["ignored"]}{COLORS['reset']} ignored error(s){COLORS['reset']}")
    if total_errors["FATAL"] > 0:
        print(f"{COLORS['FATAL']}{total_errors['FATAL']} FATAL ERRORS{COLORS['reset']}")
    print(f"{COLORS['bold']}{total_errors['total']} error(s){COLORS['reset']}, {COLORS['MAJOR']}{total_errors['MAJOR']} major{COLORS['reset']}, {COLORS['MINOR']}{total_errors['MINOR']} minor{COLORS['reset']}, {COLORS['INFO']}{total_errors['INFO']} info{COLORS['reset']}")


def style(file: str) -> None:
    total_errors = {"FATAL": 0, "MAJOR": 0, "MINOR": 0, "INFO": 0, "total": 0, "ignored": 0}
    errors = parse_error_file(file, total_errors)

    print_errors(errors)
    print_summary_errors(total_errors)

def delete_file(file: str) -> None:
    try:
        os.remove(file)
    except:
        None

def run_docker(docker_command: str) -> None:
    FILE = f"{REPORTS_DIR}/coding-style-reports.log"

    delete_file(FILE)
    os.system(f"{docker_command} run --rm --security-opt \"label:disable\" -i -v \"{DELIVERY_DIR}\":\"/mnt/delivery\" -v \"{REPORTS_DIR}\":\"/mnt/reports\" ghcr.io/epitech/coding-style-checker:latest \"/mnt/delivery\" \"/mnt/reports\"")
    style(FILE)
    if not KEEP_LOG:
        delete_file(FILE)

if __name__ == "__main__":
    docker_command = check_docker_socket()
    parser = argparse.ArgumentParser()
    parser.add_argument("delivery", nargs="?", help="The directory where your project files are", default=".")
    parser.add_argument("reports", nargs="?", help="The directory where you want the report files to be", default=".")
    parser.add_argument("--update", help="Update the CS script and the docker image", action="store_true")
    parser.add_argument("--no-ignore", help="Don't ignore files in .gitignore", action="store_true")
    parser.add_argument("-k", help="Keeps the .log file", action="store_true")
    parser.add_argument("-fc", help="Runs `make fclean` before the script", action="store_true")
    args = parser.parse_args()

    if args.update:
        update(docker_command, True)
        exit(0)
    if args.fc:
        print("Running make fclean")
        os.popen("make fclean")
    if args.no_ignore:
        IGNORE_FILES = False
    if args.k:
        KEEP_LOG = True
    if args.delivery:
        DELIVERY_DIR = read_abspath_link(args.delivery)
    if args.reports:
        REPORTS_DIR = read_abspath_link(args.reports)
    run_docker(docker_command)

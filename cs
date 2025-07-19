#!/usr/bin/env python3
import argparse
import os
from enum import Enum

INSTALL_LINK = "https://raw.githubusercontent.com/hugoarnal/cs/main/install.sh"
DOCKER_SOCKET = "/var/run/docker.sock"
DOCKER_IMAGE = "ghcr.io/epitech/coding-style-checker:latest"

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

class ErrorType(Enum):
    FATAL = "FATAL",
    MAJOR = "MAJOR",
    MINOR = "MINOR",
    INFO = "INFO",
    def __str__(self):
        return self.value[0]

COLORS = {
    ErrorType.FATAL: "\033[31m",
    ErrorType.MAJOR: "\033[31m",
    ErrorType.MINOR: "\033[93m",
    ErrorType.INFO: "\033[36m",
    "gray": "\033[90m",
    "reset": "\033[0m",
    "bold": "\033[01m"
}

def check_docker_socket() -> str:
    if os.access(DOCKER_SOCKET, os.F_OK) is False:
        raise FileNotFoundError("Docker socket not found")
    if os.access(DOCKER_SOCKET, os.R_OK) is False:
        return "sudo docker"
    else:
        return "docker"

def delete_file(file: str) -> None:
    try:
        os.remove(file)
    except:
        None

def get_ignored_files() -> str:
    return os.popen("git check-ignore $(find . -type f -printf \"%P\n\")").read()

def read_abspath_link(relpath: str) -> str:
    if os.path.islink(relpath):
        relpath = os.readlink(relpath)
    return os.path.abspath(relpath)

class Error:
    def __init__(self, error_type: ErrorType, rule: str, description: str, line: str) -> None:
        self.type: ErrorType = error_type
        self.rule: str = rule
        self.description: str = description
        self.line: str = line

class FileError:
    def __init__(self, file: str):
        self.file: str = file
        self.errors: list[Error] = []

# Returns a new file_error if not found
def file_in_array(files: list[FileError], file: str) -> tuple:
    for file_error in files:
        if file_error.file == file:
            return (True, file_error)
    return (False, FileError(file))

class CounterStyle:
    def __init__(self) -> None:
        self.delivery_dir: str = "."
        self.reports_dir: str = "."
        self.ignore: bool = True
        self.keep_log: bool = False

        self.log_file: str = ""

        self.docker: str = check_docker_socket()
        self.ignored_files: str = get_ignored_files()

        self.parse_args()
        self.run_docker()

    def parse_args(self) -> None:
        parser = argparse.ArgumentParser()
        parser.add_argument("delivery", nargs="?", help="The directory where your project files are", default=".")
        parser.add_argument("reports", nargs="?", help="The directory where you want the report files to be", default=".")
        parser.add_argument("--update", help="Update the CS script and the docker image", action="store_true")
        parser.add_argument("--no-ignore", help="Don't ignore files in .gitignore", action="store_true")
        parser.add_argument("-k", help="Keeps the .log file", action="store_true")
        parser.add_argument("-fc", help="Runs `make fclean` before the script", action="store_true")
        args = parser.parse_args()

        if args.update:
            self.update()
            exit(0)
        if args.fc:
            print("Running make fclean")
            os.popen("make fclean")
        if args.no_ignore:
            self.ignore = False
        if args.k:
            self.keep_log = True
        if args.delivery:
            self.delivery_dir = read_abspath_link(args.delivery)
        if args.reports:
            self.reports_dir = read_abspath_link(args.reports)

    def ignore_file(self, file: str) -> bool:
        for line in self.ignored_files.splitlines():
            if file == line:
                return True
        return False

    def update(self) -> None:
        if os.system(f"curl -s {INSTALL_LINK} | bash") != 0:
            exit(1)
        os.system(f"{self.docker} pull {DOCKER_IMAGE}")
        os.system(f"{self.docker} image prune -f")
        print(f"\n{COLORS['bold']}Successfully updated cs{COLORS['reset']}")

    def parse_log_file(self, total_errors: dict) -> list[FileError]:
        errors = []
        lines = open(self.log_file).read().splitlines()

        for line in lines:
            file = line.split("./")[1].split(":")[0]
            error_type_str = line.split(": ")[1].split(":")[0]
            try:
                error_type = ErrorType[error_type_str]
            except:
                raise ValueError(f"Unknown error type {error_type_str}")
            if " #" in line:
                rule = line.split(": ")[1].split(":")[1].split(" #")[0]
                description = line.split(": ")[1].split(":")[1].split(" #")[1]
            else:
                rule = line.split(": ")[1].split(":")[1]
                description = "hello world"
            line_nbr = line.split(":")[1]

            if self.ignore and self.ignore_file(file):
                total_errors["ignored"] += 1
                continue
            file_error = file_in_array(errors, file)
            if file_error[0] is False:
                errors.append(file_error[1])
            file_error[1].errors.append(Error(error_type, rule, description, line_nbr))
            total_errors[error_type] += 1
            total_errors["total"] += 1
        return errors

    def print_errors(self, errors: list[FileError]) -> None:
        for file_error in errors:
            print(f"./{file_error.file}:")
            for error in file_error.errors:
                print(f"{COLORS[error.type]}{error.type} [{error.rule}]:{COLORS['reset']} {CODING_STYLE_RULES[error.rule]} {COLORS['gray']}({file_error.file}:{error.line}){COLORS['reset']}")

    def print_summary_errors(self, total_errors: dict) -> None:
        if self.ignore and total_errors["ignored"] > 0:
            print(f"{COLORS[ErrorType.MINOR]}{total_errors['ignored']}{COLORS['reset']} ignored error(s){COLORS['reset']}")
        if total_errors[ErrorType.FATAL] > 0:
            print(f"{COLORS[ErrorType.FATAL]}{total_errors['FATAL']} FATAL ERRORS{COLORS['reset']}")
        print(f"{COLORS['bold']}{total_errors['total']} error(s){COLORS['reset']}, {COLORS[ErrorType.MAJOR]}{total_errors[ErrorType.MAJOR]} major{COLORS['reset']}, {COLORS[ErrorType.MINOR]}{total_errors[ErrorType.MINOR]} minor{COLORS['reset']}, {COLORS['INFO']}{total_errors[ErrorType.INFO]} info{COLORS['reset']}")


    def style(self) -> None:
        total_errors = {ErrorType.FATAL: 0, ErrorType.MAJOR: 0, ErrorType.MINOR: 0, ErrorType.INFO: 0, "total": 0, "ignored": 0}
        errors = self.parse_log_file(total_errors)

        self.print_errors(errors)
        self.print_summary_errors(total_errors)

    def run_docker(self) -> None:
        self.log_file = f"{self.reports_dir}/coding-style-reports.log"

        delete_file(self.log_file)
        os.system(f"{self.docker} run --rm --security-opt \"label:disable\" -i -v \"{self.delivery_dir}\":\"/mnt/delivery\" -v \"{self.reports_dir}\":\"/mnt/reports\" {DOCKER_IMAGE} \"/mnt/delivery\" \"/mnt/reports\"")
        self.style()
        if not self.keep_log:
            delete_file(self.log_file)

if __name__ == "__main__":
    CounterStyle()

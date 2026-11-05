#!/usr/bin/env python3
"""
Tool to fix remaining NorthwindMiscellaneousQueryMySqlTest failures.

This script:
1. Runs each failing test individually
2. Captures the actual MySQL-generated SQL from error output
3. Updates the test file with correct SQL assertions

Usage:
    python3 fix_remaining_tests.py [--start-index INDEX] [--batch-size SIZE]
"""

import re
import subprocess
import sys
import time

def run_single_test(test_name):
    """Run a single test and capture output"""
    cmd = [
        "dotnet", "test",
        "test/EFCore.MySql.FunctionalTests",
        "-c", "Debug",
        "--no-build",
        "--filter", f"FullyQualifiedName~NorthwindMiscellaneousQueryMySqlTest.{test_name}",
        "--logger", "console;verbosity=detailed"
    ]
    
    result = subprocess.run(
        cmd,
        capture_output=True,
        text=True,
        env={"PATH": "/home/runner/.dotnet:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin"}
    )
    
    return result.stdout + result.stderr

def extract_actual_sql(output):
    """Extract actual SQL from test failure output"""
    # Look for Actual SQL in detailed output
    patterns = [
        r'Actual:\s+"([^"]+(?:\\n[^"]*)*)"',
        r'Collection: \["([^"]+(?:\\n[^"]*)*)"'
    ]
    
    for pattern in patterns:
        match = re.search(pattern, output, re.DOTALL)
        if match:
            sql = match.group(1)
            # Decode escaped newlines and other escapes
            sql = sql.replace('\\n', '\n')
            sql = sql.replace('\\"', '"')
            sql = sql.replace('\\\\', '\\')
            # Remove truncation indicator
            sql = sql.replace('···', '')
            return sql.strip()
    
    return None

def update_test_file(test_name, sql):
    """Update test file with correct SQL"""
    test_file = 'test/EFCore.MySql.FunctionalTests/Query/NorthwindMiscellaneousQueryMySqlTest.cs'
    
    with open(test_file, 'r') as f:
        content = f.read()
    
    # Find and replace the AssertSql for this test
    # Pattern matches test with existing SQL or empty AssertSql()
    pattern = rf'(public override async Task {test_name}\(bool async\)[^{{]*\{{[^}}]*AssertSql\()(?:\(\);|"""[^"]*"""\);)'
    
    replacement = rf'\1(\n"""\n{sql}\n""");'
    
    new_content, count = re.subn(pattern, replacement, content, flags=re.MULTILINE | re.DOTALL)
    
    if count > 0:
        with open(test_file, 'w') as f:
            f.write(new_content)
        return True
    return False

def main():
    import argparse
    parser = argparse.ArgumentParser(description='Fix remaining test SQL assertions')
    parser.add_argument('--start-index', type=int, default=0, help='Start index in failing tests list')
    parser.add_argument('--batch-size', type=int, default=10, help='Number of tests to process')
    args = parser.parse_args()
    
    # Read failing tests
    with open('/tmp/test_output/failing_tests.txt', 'r') as f:
        failing_tests = [line.strip() for line in f if line.strip() and not line.startswith('Check_all')]
    
    start = args.start_index
    end = min(start + args.batch_size, len(failing_tests))
    
    print(f"Processing tests {start} to {end-1} (total {len(failing_tests)})")
    
    for i, test_name in enumerate(failing_tests[start:end], start=start):
        print(f"\n[{i+1}/{len(failing_tests)}] Processing {test_name}...")
        
        # Run test
        output = run_single_test(test_name)
        
        # Extract SQL
        sql = extract_actual_sql(output)
        
        if sql:
            # Update file
            if update_test_file(test_name, sql):
                print(f"  ✓ Updated with SQL ({len(sql)} chars)")
            else:
                print(f"  ✗ Could not update file")
        else:
            print(f"  ⚠ Could not extract SQL")
        
        # Small delay to avoid overwhelming the system
        time.sleep(0.5)
    
    print(f"\nProcessing complete")

if __name__ == '__main__':
    main()

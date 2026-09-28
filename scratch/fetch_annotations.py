import urllib.request
import re

url = "https://github.com/IT24104023/ClearToWork/actions/runs/36279074318/job/108507330529"
req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
html = urllib.request.urlopen(req).read().decode('utf-8')

# Search for annotation blocks
# Look for text inside annotations container or error messages
print("=== EXACT ERRORS REPORTED ON JOB 108507330529 ===")

# Matches file links and messages in GitHub job annotations HTML
matches = re.findall(r'<a[^>]*href="(/IT24104023/ClearToWork/blob/[^"]+)"[^>]*>(.*?)</a>', html, re.DOTALL)
for href, text in matches:
    clean_text = re.sub(r'\s+', ' ', text).strip()
    print(f"FILE: {href}\nTEXT: {clean_text}\n")

# Also look for error text blocks
error_blocks = re.findall(r'<div[^>]*class="[^"]*annotation[^"]*"[^>]*>(.*?)</div>', html, re.DOTALL)
for b in error_blocks:
    print("ANNOTATION:", re.sub(r'\s+', ' ', b).strip())

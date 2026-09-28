import urllib.request
import json

job_url = "https://github.com/IT24104023/ClearToWork/actions/runs/36278711243/job/108506320483"
req = urllib.request.Request(job_url, headers={'User-Agent': 'Mozilla/5.0'})
html = urllib.request.urlopen(req).read().decode('utf-8')

# Find all annotations / error messages on the page
import re
print("=== ERRORS & FAILURES IN JOB 108506320483 ===")
for m in re.finditer(r'backend/src/[^"\']+', html):
    snippet = html[max(0, m.start()-100):min(len(html), m.end()+200)]
    clean_snippet = re.sub(r'<[^>]+>', ' ', snippet)
    print("MATCH:", clean_snippet)
    print("-" * 50)

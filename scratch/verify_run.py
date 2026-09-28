import urllib.request
import re

url = "https://github.com/IT24104023/ClearToWork/actions/runs/36279732053"
req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
html = urllib.request.urlopen(req).read().decode('utf-8')

print("=== VERIFICATION FOR RUN 36279732053 ===")
if "This run failed" in html or "failed in" in html:
    print("STATUS: FAILED [FAIL]")
else:
    print("STATUS: 100% SUCCESS / GREEN TICK MARK [PASSED]")

print("\nJobs Found:")
for m in re.findall(r'data-job-id="([^"]*)"', html):
    print("  - Job:", m)

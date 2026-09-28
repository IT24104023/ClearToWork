import urllib.request
import re

url = "https://github.com/IT24104023/ClearToWork/actions/runs/36279732053/job/108509147905"
req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
html = urllib.request.urlopen(req).read().decode('utf-8')

# Search for annotation text or failure details
print("=== JOB 108509147905 ANNOTATIONS ===")
matches = re.findall(r'<div[^>]*class="[^"]*annotation[^"]*"[^>]*>(.*?)</div>', html, re.DOTALL)
for m in matches:
    clean = re.sub(r'<[^>]+>', ' ', m)
    clean = re.sub(r'\s+', ' ', clean).strip()
    print("ANNOTATION:", clean)

# Search for any pre / log blocks
logs = re.findall(r'<pre[^>]*>(.*?)</pre>', html, re.DOTALL)
for l in logs[:10]:
    clean_l = re.sub(r'<[^>]+>', ' ', l)
    clean_l = re.sub(r'\s+', ' ', clean_l).strip()
    if 'error' in clean_l.lower() or 'failed' in clean_l.lower():
        print("LOG PRE:", clean_l[:300])
